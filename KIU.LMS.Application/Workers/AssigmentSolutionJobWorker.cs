using System.Text.Json;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Workers;

public class AssignmentSolutionJobWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AssignmentSolutionJobWorker> _logger;

    public AssignmentSolutionJobWorker(IServiceScopeFactory scopeFactory, ILogger<AssignmentSolutionJobWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AssignmentSolutionJobWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobRepo = scope.ServiceProvider.GetRequiredService<IAssignmentSolutionJobRepository>();
            var aiService = scope.ServiceProvider.GetRequiredService<IAiGradingService>();
            var assignmentRepo = scope.ServiceProvider.GetRequiredService<IAssignmentRepository>();
            var solutionRepo = scope.ServiceProvider.GetRequiredService<ISolutionRepository>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var jobs = await jobRepo.GetPendingJobsAsync(stoppingToken);

            foreach (var job in jobs)
            {
                try
                {
                    job.IncrementAttempts(job.Id);

                    var assignment =
                        await assignmentRepo.FirstOrDefaultWithTrackingAsync(j => j.Id == job.AssignmentId,
                            stoppingToken);
                    var solution =
                        await solutionRepo.FirstOrDefaultWithTrackingAsync(j => j.Id == job.SolutionId, stoppingToken);

                    if (assignment == null || solution == null)
                        throw new Exception("Assignment or Solution not found");

                    var problem = assignment.Problem ?? string.Empty;
                    var reference = assignment.CodeSolution ?? string.Empty;
                    var rubric = assignment.PromptText ?? "No rubric provided.";
                    var studentSubmission = solution.Value;

                    if (string.IsNullOrWhiteSpace(problem) || string.IsNullOrWhiteSpace(reference))
                        throw new Exception("Invalid assignment data for grading.");

                    var gradeResult = await aiService.GradeAsync(problem, reference, studentSubmission, rubric, assignment.Score ?? 10);
                    if (gradeResult == null)
                    {
                        solution.Failed(""); 
                        job.MarkAsFailed(job.Id);
                    }
                    else
                    {
                        try
                        {
                            var doc = JsonDocument.Parse(gradeResult);
                            var grade = doc.RootElement.GetProperty("grade").GetDecimal().ToString();
                            var feedback = doc.RootElement.GetProperty("feedback").GetString() ?? "";

                            if (assignment.ValidationsCount > 0)
                            {
                                for (var i = 0; i < assignment.ValidationsCount; i++)
                                {
                                    var result = await aiService.ValidateAsync(problem, reference, studentSubmission, rubric, gradeResult);

                                    if (!result.isValid)
                                        solution.Failed("");
                                
                                    else
                                    {
                                        solution.Graded(grade, feedback);
                                        break;
                                    }
                                }
                            }
                            
                            solution.Graded(grade, feedback);
                            job.MarkAsGraded(gradeResult, job.Id);
                        }
                        catch (JsonException ex)
                        {
                            if (ex != null) _logger.LogInformation(ex.Message);
                            solution.Failed("");
                            job.MarkAsFailed(job.Id);
                        }
                    }

                    _logger.LogInformation("Job {JobId} processed successfully", job.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job {JobId} failed attempt {Attempt}", job.Id, job.Attempts);

                    if (job.Attempts >= 3)
                    {
                        job.MarkAsFailed(job.Id);
                        _logger.LogWarning("Job {JobId} permanently failed after 3 attempts", job.Id);
                    }
                }
            }

            await uow.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}