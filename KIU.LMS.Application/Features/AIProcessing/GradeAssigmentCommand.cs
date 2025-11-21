using System.Text.Json;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.Extensions.Logging;

public record GradeAssignmentJobCommand(Guid JobId) : IRequest<AIProcessingResult>;

public class GradeAssignmentJobCommandHandler 
    : IRequestHandler<GradeAssignmentJobCommand, AIProcessingResult>
{
    private readonly IAssignmentSolutionJobRepository _jobRepo;
    private readonly IAiGradingService _aiService;
    private readonly IAssignmentRepository _assignmentRepo;
    private readonly ISolutionRepository _solutionRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GradeAssignmentJobCommandHandler> _logger;

    public GradeAssignmentJobCommandHandler(
        IAssignmentSolutionJobRepository jobRepo,
        IAiGradingService aiService,
        IAssignmentRepository assignmentRepo,
        ISolutionRepository solutionRepo,
        IUnitOfWork uow,
        ILogger<GradeAssignmentJobCommandHandler> logger)
    {
        _jobRepo = jobRepo;
        _aiService = aiService;
        _assignmentRepo = assignmentRepo;
        _solutionRepo = solutionRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<AIProcessingResult> Handle(
        GradeAssignmentJobCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepo.FirstOrDefaultWithTrackingAsync(
            x => x.Id == request.JobId, cancellationToken);

        if (job == null)
        {
            return new AIProcessingResult(false, "{}", "Job not found");
        }

        var assignment = await _assignmentRepo.FirstOrDefaultWithTrackingAsync(
            a => a.Id == job.AssignmentId, cancellationToken);

        var solution = await _solutionRepo.FirstOrDefaultWithTrackingAsync(
            s => s.Id == job.SolutionId, cancellationToken);

        if (assignment == null || solution == null)
        {
            job.MarkAsFailed(job.Id);
            await _uow.SaveChangesAsync();

            return new AIProcessingResult(false, "{}", "Assignment or Solution missing");
        }

        try
        {
            job.IncrementAttempts(job.Id);

            var gradeResult = await _aiService.GradeAsync(
                assignment.Problem ?? "",
                assignment.CodeSolution ?? "",
                solution.Value,
                assignment.PromptText ?? "",
                assignment.Score ?? 10);

            if (gradeResult == null)
            {
                return FailOrRetry(job, solution, "Empty grade result");
            }

            JsonElement json;
            try
            {
                json = JsonDocument.Parse(gradeResult).RootElement;
            }
            catch
            {
                return FailOrRetry(job, solution, "Invalid JSON format");
            }

            if (!TryParseGrade(json, out int grade))
            {
                return FailOrRetry(job, solution, "Invalid grade format");
            }

            if (grade < 0 || grade > (assignment.Score ?? 10))
            {
                return FailOrRetry(job, solution, "Grade out of valid range");
            }

            string feedback =
                json.TryGetProperty("feedback", out var fb)
                    ? fb.GetString() ?? ""
                    : "";

            solution.Graded(grade.ToString(), feedback);
            job.MarkAsGraded(gradeResult, job.Id);

            await _uow.SaveChangesAsync();

            return new AIProcessingResult(true, gradeResult, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobId} crashed on attempt {Attempt}", job.Id, job.Attempts);
            return FailOrRetry(job, solution!, ex.Message);
        }
    }

    private AIProcessingResult FailOrRetry(AssignmentSolutionJob job, Solution solution, string message)
    {
        _logger.LogWarning("Grading Failed: {Msg}. Job {JobId} Attempt {Attempt}",
            message, job.Id, job.Attempts);

        if (job.Attempts < 3)
        {
            _uow.SaveChangesAsync();

            return new AIProcessingResult(
                false,
                "{}",
                "Retrying grade"
            );
        }

        solution.Failed(message);
        job.MarkAsFailed(job.Id);
        _uow.SaveChangesAsync().Wait();

        return new AIProcessingResult(
            false,
            "{}",
            message
        );
    }

    private bool TryParseGrade(JsonElement json, out int grade)
    {
        grade = 0;

        if (!json.TryGetProperty("grade", out var gradeProp))
            return false;

        return gradeProp.ValueKind == JsonValueKind.Number
            ? (grade = gradeProp.GetInt32()) >= 0
            : (gradeProp.ValueKind == JsonValueKind.String
               && int.TryParse(gradeProp.GetString(), out grade));
    }
}
