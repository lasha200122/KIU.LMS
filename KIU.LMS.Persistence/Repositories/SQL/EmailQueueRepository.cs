namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class EmailQueueRepository(LmsDbContext dbContext)
   : BaseRepository<EmailQueue>(dbContext), IEmailQueueRepository
{ }