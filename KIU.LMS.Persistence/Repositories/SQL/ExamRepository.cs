namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class ExamRepository(LmsDbContext dbContext)
   : BaseRepository<Exam>(dbContext), IExamRepository
{ }