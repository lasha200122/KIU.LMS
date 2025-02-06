namespace KIU.LMS.Persistence.Repositories.SQL;

public class ExamResultRepository(LmsDbContext db) : BaseRepository<ExamResult>(db), IExamResultRepository
{
}
