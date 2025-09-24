using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;


public sealed class LoginAttemptRepository(LmsDbContext _context)
   : BaseRepository<LoginAttempt>(_context), ILoginAttemptRepository
{
    public async Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(Guid userId)
    {
        return await _context.LoginAttempts
            .Where(la => la.UserId == userId)
            .OrderByDescending(la => la.CreateDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<LoginAttempt>> GetRecentAttemptsAsync(Guid userId, int count = 10)
    {
        return await _context.LoginAttempts
            .Where(la => la.UserId == userId)
            .OrderByDescending(la => la.CreateDate)
            .Take(count)
            .ToListAsync();
    }
}