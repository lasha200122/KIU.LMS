namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class CourseRepository(LmsDbContext dbContext)
    : BaseRepository<Course>(dbContext), ICourseRepository 
{
}