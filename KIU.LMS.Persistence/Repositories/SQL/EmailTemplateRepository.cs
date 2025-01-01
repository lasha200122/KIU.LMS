namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class EmailTemplateRepository(LmsDbContext dbContext)
   : BaseRepository<EmailTemplate>(dbContext), IEmailTemplateRepository
{ }