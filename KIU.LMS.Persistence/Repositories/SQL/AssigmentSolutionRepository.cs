namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class AssignmentSolutionJobRepository(LmsDbContext context) : 
    BaseRepository<AssignmentSolutionJob>(context), IAssignmentSolutionJobRepository
{
    public async Task<List<AssignmentSolutionJob>> GetPendingJobsAsync(CancellationToken cancellationToken)
    {
        return await context.AssignmentSolutionJobs
            .Where(j => j.Status == AssignmentSolutionJobStatus.Pending && j.Attempts < 3)
            .ToListAsync(cancellationToken);
    }
}