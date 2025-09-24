using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public class QuizRepository(LmsDbContext _context) : BaseRepository<Quiz>(_context), IQuizRepository
{
    public async Task<Quiz?> GetByIdAsync(Guid id)
    {
        return await _context.Quizzes
            .Include(q => q.Course)
            .Include(q => q.ExamResults)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quiz?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Quizzes
            .Include(q => q.Course)
                .ThenInclude(c => c.UserCourses)
            .Include(q => q.ExamResults)
                .ThenInclude(er => er.User)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Quizzes
            .Include(q => q.ExamResults)
            .Where(q => q.CourseId == courseId)
            .OrderBy(q => q.StartDateTime)
            .ToListAsync();
    }
}
