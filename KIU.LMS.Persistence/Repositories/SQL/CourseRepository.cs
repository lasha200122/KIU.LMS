namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class CourseRepository(LmsDbContext _context)
    : BaseRepository<Course>(_context), ICourseRepository 
{
    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.UserCourses)
            .Include(c => c.Topics)
            .Include(c => c.Assignments)
            .Include(c => c.Modules)
            .Include(c => c.Quizzes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.UserCourses)
                .ThenInclude(uc => uc.User)
            .Include(c => c.Materials)
            .Include(c => c.Meetings)
            .Include(c => c.Topics)
                .ThenInclude(t => t.Assignments)
            .Include(c => c.Assignments)
                .ThenInclude(a => a.Solutions)
            .Include(c => c.Modules)
                .ThenInclude(m => m.ModuleBanks)
                .ThenInclude(mb => mb.SubModules)
            .Include(c => c.Quizzes)
                .ThenInclude(q => q.ExamResults)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Courses
            .Include(c => c.UserCourses)
            .Include(c => c.Topics)
            .Include(c => c.Assignments)
            .Include(c => c.Modules)
            .Where(c => c.UserCourses.Any(uc => uc.UserId == userId))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}