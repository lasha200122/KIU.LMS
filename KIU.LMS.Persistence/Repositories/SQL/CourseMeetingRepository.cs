namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class CourseMeetingRepository(LmsDbContext dbContext)
   : BaseRepository<CourseMeeting>(dbContext), ICourseMeetingRepository
{
}