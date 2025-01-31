namespace KIU.LMS.Persistence.Repositories.SQL;

public class SolutionRepository(LmsDbContext context): BaseRepository<Solution>(context) , ISolutionRepository
{
}
