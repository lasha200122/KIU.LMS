namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class AssignmentRepository(LmsDbContext context) : BaseRepository<Assignment>(context), IAssignmentRepository
{
}
