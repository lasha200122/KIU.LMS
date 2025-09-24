using KIU.LMS.Domain.Common.Enums.User;

namespace KIU.LMS.Persistence.Repositories.SQL;


public sealed class UserRepository(LmsDbContext _context)
   : BaseRepository<User>(_context), IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserCourses)
                .ThenInclude(uc => uc.Course)
            .Include(u => u.LoginAttempts)
            .Include(u => u.Devices)
            .Include(u => u.Solutions)
            .Include(u => u.ExamResults)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserCourses)
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetStudentsByCourseIdAsync(Guid courseId)
    {
        return await _context.Users
            .Include(u => u.UserCourses)
            .Where(u => u.Role == UserRole.Student &&
                       u.UserCourses.Any(uc => uc.CourseId == courseId))
            .ToListAsync();
    }
}