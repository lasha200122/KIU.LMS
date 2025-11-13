using KIU.LMS.Domain.Common.Enums.Assignment;

namespace KIU.LMS.Persistence.Repositories.SQL;

public class GeneratedAssignmentRepository(LmsDbContext db)
    : BaseRepository<GeneratedAssignment>(db), IGeneratedAssignmentRepository
{
    public async Task<GeneratedAssignment?> GetMCQInProgressAsync(CancellationToken ct = default)
    {
        return await db.Set<GeneratedAssignment>()
            .FirstOrDefaultAsync(a => 
                a.Status == GeneratingStatus.InProgress && a.Type == GeneratedAssignmentType.MCQ, ct);
    }
}