using KIU.LMS.Domain.Common.Enums.Assignment;

namespace KIU.LMS.Persistence.Repositories.SQL;

public class GeneratedAssignmentRepository(LmsDbContext db)
    : BaseRepository<GeneratedAssignment>(db), IGeneratedAssignmentRepository
{
    public async Task<GeneratedAssignment?> GetMCQInProgressAsync(CancellationToken ct = default)
    {
        return await db.Set<GeneratedAssignment>()
            .FromSqlRaw(sql: """
                             SELECT * FROM "GeneratedAssignment"
                             WHERE "Status" = 1 AND "GeneratedAssignmentType" = 1 
                             FOR UPDATE 
                             """)
            .FirstOrDefaultAsync(ct);
    }
}