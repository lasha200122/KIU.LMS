namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class TopicRepository(LmsDbContext context) : BaseRepository<Topic>(context) , ITopicRepository
{
}
