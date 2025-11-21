namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IVotingSessionRepository : IBaseRepository<VotingSession>
{
    public Task<Dictionary<Guid, int>> GetResultsAsync(Guid sessionId);
    public Task<VotingSession?> GetByIdWithOptionsAsync(Guid id, CancellationToken ct);
}
