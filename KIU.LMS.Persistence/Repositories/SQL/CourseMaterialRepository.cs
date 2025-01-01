namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class CourseMaterialRepository(LmsDbContext dbContext)
   : BaseRepository<CourseMaterial>(dbContext), ICourseMaterialRepository
{ 
}