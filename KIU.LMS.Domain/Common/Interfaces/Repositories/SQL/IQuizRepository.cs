namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<Quiz?> GetByIdAsync(Guid id);
    Task<Quiz?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Quiz>> GetByCourseIdAsync(Guid courseId);
}
