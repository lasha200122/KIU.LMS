using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public class ExamResultRepository(LmsDbContext context) : BaseRepository<ExamResult>(context), IExamResultRepository
{
    public async Task<IEnumerable<ExamResult>> GetByStudentIdAsync(Guid studentId)
    {
        return await context.ExamResults
            .Include(e => e.Quiz)
            .Include(e => e.User)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.FinishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExamResult>> GetByQuizIdAsync(Guid quizId)
    {
        return await context.ExamResults
            .Where(e => e.QuizId == quizId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExamResult>> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await context.ExamResults
            .Include(e => e.Quiz)
            .Where(e => e.StudentId == userId && e.Quiz.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<ExamResult?> GetBySessionIdAsync(string sessionId)
    {
        return await context.ExamResults
            .FirstOrDefaultAsync(e => e.SessionId == sessionId);
    }
}
