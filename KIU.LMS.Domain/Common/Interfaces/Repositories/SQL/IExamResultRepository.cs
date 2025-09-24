namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IExamResultRepository : IBaseRepository<ExamResult>
{
    Task<IEnumerable<ExamResult>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<ExamResult>> GetByQuizIdAsync(Guid quizId);
    Task<IEnumerable<ExamResult>> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<ExamResult?> GetBySessionIdAsync(string sessionId);
}
