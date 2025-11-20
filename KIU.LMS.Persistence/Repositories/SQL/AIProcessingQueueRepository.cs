public class AIProcessingQueueRepository(LmsDbContext context)
    : BaseRepository<AIProcessingQueue>(context), IAIProcessingQueueRepository
{
    public async Task<AIProcessingQueue[]> GetPendingJobsAsync(CancellationToken ct)
    {
        return await context
            .Set<AIProcessingQueue>()
            .Where(x => x.Status == AIProcessingStatus.Pending)
            .OrderBy(x => x.CreateDate)
            .ToArrayAsync(ct);
    }
}