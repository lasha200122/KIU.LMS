namespace KIU.LMS.Persistence.Repositories.SQL;

public class GeneratedAssignmentRepository(LmsDbContext db)
    : BaseRepository<GeneratedAssignment>(db), IGeneratedAssignmentRepository
{
    public async Task<GeneratedAssignment?> GetInProgressAsync(CancellationToken ct = default)
    {
        return await db.Set<GeneratedAssignment>()
            .FirstOrDefaultAsync(a => a.Status == GeneratingStatus.InProgress, ct);
    }
}