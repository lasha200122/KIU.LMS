namespace KIU.LMS.Persistence.Repositories.SQL;

public class VotingSessionRepository(LmsDbContext context) :
    BaseRepository<VotingSession>(context), IVotingSessionRepository
{
    public async Task<Dictionary<Guid, int>> GetResultsAsync(Guid sessionId)
    {
        return await context.Votes
            .Where(v => v.SessionId == sessionId)
            .GroupBy(v => v.OptionId)
            .Select(g => new { OptionId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OptionId, x => x.Count);
    }
    
    public async Task<VotingSession?> GetByIdWithOptionsAsync(Guid id, CancellationToken ct)
    {
        return await context.VotingSessions
            .Include(s => s.Options)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}