using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class UserCourseRepository(LmsDbContext _context)
   : BaseRepository<UserCourse>(_context), IUserCourseRepository
{
    public async Task<IEnumerable<UserCourse>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserCourses
            .Include(uc => uc.Course)
            .Where(uc => uc.UserId == userId)
            .OrderByDescending(uc => uc.CreateDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserCourse>> GetByUserIdWithDetailsAsync(Guid userId)
    {
        return await _context.UserCourses
            .Include(uc => uc.Course)
                .ThenInclude(c => c.Topics)
            .Include(uc => uc.Course)
                .ThenInclude(c => c.Assignments)
            .Include(uc => uc.Course)
                .ThenInclude(c => c.Modules)
            .Include(uc => uc.Course)
                .ThenInclude(c => c.Quizzes)
            .Include(uc => uc.User)
            .Where(uc => uc.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserCourse>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.UserCourses
            .Include(uc => uc.User)
            .Where(uc => uc.CourseId == courseId)
            .OrderBy(uc => uc.User.LastName)
            .ThenBy(uc => uc.User.FirstName)
            .ToListAsync();
    }
}