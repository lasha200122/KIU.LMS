namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class QuestionBankRepository(LmsDbContext dbContext)
   : BaseRepository<QuestionBank>(dbContext), IQuestionBankRepository
{ }