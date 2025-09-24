namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface ICourseRepository : IBaseRepository<Course> 
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Course>> GetByUserIdAsync(Guid userId);
}