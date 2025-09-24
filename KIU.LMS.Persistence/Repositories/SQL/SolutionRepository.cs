namespace KIU.LMS.Persistence.Repositories.SQL;

public class SolutionRepository(LmsDbContext context): BaseRepository<Solution>(context) , ISolutionRepository
{
    public async Task<IEnumerable<Solution>> GetByUserIdAsync(Guid userId)
    {
        return await context.Solutions
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solution>> GetByUserIdWithDetailsAsync(Guid userId)
    {
        return await context.Solutions
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Topic)
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solution>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await context.Solutions
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solution>> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await context.Solutions
            .Include(s => s.Assignment)
            .Where(s => s.UserId == userId && s.Assignment.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<Solution?> GetByUserAndAssignmentAsync(Guid userId, Guid assignmentId)
    {
        return await context.Solutions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.AssignmentId == assignmentId);
    }
}
