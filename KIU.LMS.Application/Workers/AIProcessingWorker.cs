using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.Extensions.Logging;

public class AIProcessingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AIProcessingWorker> _logger;

    public AIProcessingWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AIProcessingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AIProcessingWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var queueRepo = scope.ServiceProvider.GetRequiredService<IAIProcessingQueueRepository>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var jobs = await queueRepo.GetPendingJobsAsync(stoppingToken);

                var jobToProcess =
                    jobs.FirstOrDefault(x => x.Type == AIProcessingType.Grading) ??
                    (IsNightTime() ? jobs.FirstOrDefault(x => x.Type == AIProcessingType.MCQ) : null) ??
                    (IsNightTime() ? jobs.FirstOrDefault(x =>
                        x.Type is AIProcessingType.C2RS or AIProcessingType.IPEQ) : null);

                if (jobToProcess != null)
                {
                    await ProcessJob(jobToProcess, mediator, uow, stoppingToken);
                }
                else
                {
                    await Task.Delay(30000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AIProcessingWorker loop crashed");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessJob(
        AIProcessingQueue job,
        IMediator mediator,
        IUnitOfWork uow,
        CancellationToken ct)
    {
        _logger.LogInformation("Processing job {Id} type {Type}", job.Id, job.Type);
        job.Start();
        await uow.SaveChangesAsync();
        
        var result = job.Type switch
        {
            AIProcessingType.Grading =>
                await mediator.Send(new GradeAssignmentJobCommand(job.TargetId), ct),

            AIProcessingType.C2RS or AIProcessingType.IPEQ =>
                await mediator.Send(new GenerateTaskAssignmentCommand(job.TargetId), ct),

            AIProcessingType.MCQ =>
                await mediator.Send(new GenerateAssignmentCommand(job.TargetId), ct),

            _ => new AIProcessingResult(false, "{}", "Unsupported type")
        };

        if (result.Success)
            job.MarkCompleted(result.ResultJson);
        else if (result.ErrorMessage == "Retrying grade")
            job.MarkToRetry();
        else
            job.MarkFailed(result.ErrorMessage ?? "Unknown error");

        await uow.SaveChangesAsync();
    }

    private static bool IsNightTime()
    {
        var hour = DateTimeOffset.UtcNow.Hour;
        return hour is >= 0 and < 6;
    }
}
