namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IUserRepository : IBaseRepository<User> 
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetStudentsByCourseIdAsync(Guid courseId);
}
