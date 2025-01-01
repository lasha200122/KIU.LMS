namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class ExamQuestionRepository(LmsDbContext dbContext)
   : BaseRepository<ExamQuestion>(dbContext), IExamQuestionRepository
{ }