namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface ITopicRepository : IBaseRepository<Topic>
{
    Task<IEnumerable<Topic>> GetByCourseIdAsync(Guid courseId);
    Task<Topic?> GetByIdAsync(Guid id);
}
