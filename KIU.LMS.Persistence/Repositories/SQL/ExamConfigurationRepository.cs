namespace KIU.LMS.Persistence.Repositories.SQL;


public sealed class ExamConfigurationRepository(LmsDbContext dbContext)
   : BaseRepository<ExamConfiguration>(dbContext), IExamConfigurationRepository
{ }