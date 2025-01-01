namespace KIU.LMS.Persistence.Repositories.SQL;


public sealed class UserRepository(LmsDbContext dbContext)
   : BaseRepository<User>(dbContext), IUserRepository
{ }