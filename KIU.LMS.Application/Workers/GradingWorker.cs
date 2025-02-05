namespace KIU.LMS.Application.Workers;

public class GradingWorker(IServiceProvider _serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingAssignmentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessPendingAssignmentsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var _grader = scope.ServiceProvider.GetRequiredService<IGradingService>();

        var currentTime = DateTimeOffset.UtcNow;

        var solutions = await _unitOfWork.SolutionRepository.GetWhereAsTrackingIncludedAsync(x => 
        x.Assignment.AIGrader && 
        (!x.Assignment.EndDateTime.HasValue || x.Assignment.EndDateTime.Value < currentTime) &&
        (x.GradingStatus == GradingStatus.None || x.GradingStatus == GradingStatus.Failed), x => x.Assignment, x => x.Assignment.Prompt);

        foreach (var solution in solutions)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await _grader.GradeSubmissionAsync(solution);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
