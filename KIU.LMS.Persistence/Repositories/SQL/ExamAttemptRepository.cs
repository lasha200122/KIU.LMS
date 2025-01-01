namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class ExamAttemptRepository(LmsDbContext dbContext)
   : BaseRepository<ExamAttempt>(dbContext), IExamAttemptRepository
{ }