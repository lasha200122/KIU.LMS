namespace KIU.LMS.Persistence.Repositories.SQL;

public class VotingOptionRepository(LmsDbContext context)
    : BaseRepository<VotingOption>(context), IVotingOptionRepository { }