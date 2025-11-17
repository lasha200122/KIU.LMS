namespace KIU.LMS.Persistence.Repositories.SQL;

public class GeneratedTaskRepository(LmsDbContext db)
    : BaseRepository<GeneratedTask>(db), IGeneratedTaskRepository
{
    public async Task<GeneratedAssignment?> GetTaskInProgressAsync(CancellationToken ct = default)
    {
        return await db.Set<GeneratedAssignment>()
            .FromSqlRaw(sql: """
                   SELECT * FROM "GeneratedAssignment"
                   WHERE "Status" = 1 AND ("Type" = 2 OR "Type" = 3)
                   FOR UPDATE 
                   """)
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddSafetyAsync(GeneratedTask task, int maxCount, Guid assignmentId, CancellationToken ct = default)
    {
        var currentCount = await db.Set<GeneratedTask>()
            .Where(t => t.GeneratedAssignmentId == assignmentId)
            .CountAsync(ct);

        if (currentCount < maxCount)
        {
            await db.Set<GeneratedTask>().AddAsync(task, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}
