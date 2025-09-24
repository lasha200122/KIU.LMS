namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IUserDeviceRepository : IBaseRepository<UserDevice> 
{
    Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId);

}