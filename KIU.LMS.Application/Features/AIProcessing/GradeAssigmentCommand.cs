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
            return new AIProcessingResult(
                false,
                "{}",
                "Job not found"
            );
        }

        try
        {
            job.IncrementAttempts(job.Id);

            var assignment = await _assignmentRepo.FirstOrDefaultWithTrackingAsync(
                a => a.Id == job.AssignmentId, cancellationToken);

            var solution = await _solutionRepo.FirstOrDefaultWithTrackingAsync(
                s => s.Id == job.SolutionId, cancellationToken);

            if (assignment == null || solution == null)
            {
                job.MarkAsFailed(job.Id);
                await _uow.SaveChangesAsync();

                return new AIProcessingResult(
                    false,
                    "{}",
                    "Assignment or Solution not found"
                );
            }

            var gradeResult = await _aiService.GradeAsync(
                assignment.Problem ?? "",
                assignment.CodeSolution ?? "",
                solution.Value,
                assignment.PromptText ?? "",
                assignment.Score ?? 10);

            if (gradeResult == null)
            {
                solution.Failed();
                job.MarkAsFailed(job.Id);
            }
            else
            {
                try
                {
                    var json = JsonDocument.Parse(gradeResult).RootElement;
                    var grade = json.GetProperty("grade").GetInt32().ToString();
                    var feedback = json.GetProperty("feedback").GetString() ?? "";

                    if (assignment.ValidationsCount > 0)
                    {
                        for (var i = 0; i < assignment.ValidationsCount; i++)
                        {
                            var validation = await _aiService.ValidateAsync(
                                assignment.Problem ?? "",
                                assignment.CodeSolution ?? "",
                                solution.Value,
                                assignment.PromptText ?? "",
                                gradeResult);

                            if (!validation.isValid)
                            {
                                solution.Failed(validation.reason);
                                continue;
                            }

                            solution.Graded(grade, feedback);
                            break;
                        }
                    }
                    else
                    {
                        solution.Graded(grade, feedback);
                    }

                    job.MarkAsGraded(gradeResult, job.Id);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogInformation(jsonEx.Message);
                    solution.Failed("Invalid JSON format");
                    job.MarkAsFailed(job.Id);
                }
            }

            await _uow.SaveChangesAsync();

            return new AIProcessingResult(
                job.Status == AssignmentSolutionJobStatus.Graded,
                gradeResult ?? "{}",
                job.Status == AssignmentSolutionJobStatus.Failed ? "Failed" : null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobId} failed attempt {Attempts}", job.Id, job.Attempts);

            job.MarkAsFailed(job.Id);
            await _uow.SaveChangesAsync();

            return new AIProcessingResult(
                false,
                "{}",
                ex.Message
            );
        }
    }
}