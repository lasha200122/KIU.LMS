namespace KIU.LMS.Persistence.Repositories.SQL;

public class FileRepository(LmsDbContext db) : BaseRepository<FileRecord>(db), IFileRepository
{
}
