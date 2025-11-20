namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IAIProcessingQueueRepository : IBaseRepository<AIProcessingQueue>
{
    Task<AIProcessingQueue[]> GetPendingJobsAsync(CancellationToken cancellationToken);
}