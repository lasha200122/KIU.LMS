namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IAssignmentSolutionJobRepository : IBaseRepository<AssignmentSolutionJob>
{
    Task<List<AssignmentSolutionJob>> GetPendingJobsAsync(CancellationToken cancellationToken);
}

