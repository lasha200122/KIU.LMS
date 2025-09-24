namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface ILoginAttemptRepository : IBaseRepository<LoginAttempt> 
{
    Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<LoginAttempt>> GetRecentAttemptsAsync(Guid userId, int count = 10);
}
