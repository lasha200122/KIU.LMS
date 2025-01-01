namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class UserDeviceRepository(LmsDbContext dbContext)
   : BaseRepository<UserDevice>(dbContext), IUserDeviceRepository
{ }