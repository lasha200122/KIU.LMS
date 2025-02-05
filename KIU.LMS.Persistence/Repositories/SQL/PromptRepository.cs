namespace KIU.LMS.Persistence.Repositories.SQL;

public class PromptRepository(LmsDbContext db) : BaseRepository<Prompt>(db), IPromptRepository
{
}
