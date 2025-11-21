namespace KIU.LMS.Persistence.Repositories.SQL;

public class VoteRepository(LmsDbContext context) : BaseRepository<Vote>(context), IVoteRepository 
{
    public async Task<bool> HasUserVotedAsync(Guid sessionId, Guid userId, CancellationToken ct)
    {
        return await context.Set<Vote>()
            .AnyAsync(v => v.SessionId == sessionId && v.UserId == userId, ct);
    }
}