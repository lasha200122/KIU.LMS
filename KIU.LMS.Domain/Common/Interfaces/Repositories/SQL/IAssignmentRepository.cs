namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IAssignmentRepository : IBaseRepository<Assignment>
{
    Task<Assignment?> GetByIdAsync(Guid id);
    Task<Assignment?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Assignment>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<Assignment>> GetByTopicIdAsync(Guid topicId);
}
