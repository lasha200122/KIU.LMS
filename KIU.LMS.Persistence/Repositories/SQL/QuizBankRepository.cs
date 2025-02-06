namespace KIU.LMS.Persistence.Repositories.SQL;

public class QuizBankRepository(LmsDbContext db) : BaseRepository<QuizBank>(db), IQuizBankRepository
{
}
