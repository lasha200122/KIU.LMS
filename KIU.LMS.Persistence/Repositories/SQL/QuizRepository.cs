namespace KIU.LMS.Persistence.Repositories.SQL;

public class QuizRepository(LmsDbContext db) : BaseRepository<Quiz>(db), IQuizRepository
{
}
