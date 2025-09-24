namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface ISolutionRepository : IBaseRepository<Solution>
{
    Task<IEnumerable<Solution>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Solution>> GetByUserIdWithDetailsAsync(Guid userId);
    Task<IEnumerable<Solution>> GetByAssignmentIdAsync(Guid assignmentId);
    Task<IEnumerable<Solution>> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<Solution?> GetByUserAndAssignmentAsync(Guid userId, Guid assignmentId);
}
