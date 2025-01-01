namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class UserCourseRepository(LmsDbContext dbContext)
   : BaseRepository<UserCourse>(dbContext), IUserCourseRepository
{ }