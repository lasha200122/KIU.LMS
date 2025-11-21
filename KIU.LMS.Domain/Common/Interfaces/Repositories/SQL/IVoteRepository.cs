namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IVoteRepository : IBaseRepository<Vote>
{
    public Task<bool> HasUserVotedAsync(Guid sessionId, Guid userId, CancellationToken ct);
}