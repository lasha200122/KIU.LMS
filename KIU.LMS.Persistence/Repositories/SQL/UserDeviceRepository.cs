namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class UserDeviceRepository(LmsDbContext dbContext)
   : BaseRepository<UserDevice>(dbContext), IUserDeviceRepository
{
    public async Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId)
    {
        return await dbContext.UserDevices
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.LastUpdateDate)
            .ToListAsync();
    }
}