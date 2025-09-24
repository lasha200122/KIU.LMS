namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IUserCourseRepository : IBaseRepository<UserCourse>
{
    Task<IEnumerable<UserCourse>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserCourse>> GetByUserIdWithDetailsAsync(Guid userId);
    Task<IEnumerable<UserCourse>> GetByCourseIdAsync(Guid courseId);
}
