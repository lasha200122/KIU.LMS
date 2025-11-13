namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IGeneratedAssignmentRepository : IBaseRepository<GeneratedAssignment>
{
    Task<GeneratedAssignment?> GetMCQInProgressAsync(CancellationToken ct = default);
}
