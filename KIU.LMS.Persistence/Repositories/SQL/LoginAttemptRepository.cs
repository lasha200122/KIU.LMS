namespace KIU.LMS.Persistence.Repositories.SQL;


public sealed class LoginAttemptRepository(LmsDbContext dbContext)
   : BaseRepository<LoginAttempt>(dbContext), ILoginAttemptRepository
{ }