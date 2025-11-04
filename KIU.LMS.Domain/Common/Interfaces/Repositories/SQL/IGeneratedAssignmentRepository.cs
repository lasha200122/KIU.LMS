namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IGeneratedAssignmentRepository : IBaseRepository<GeneratedAssignment>
{
    Task<GeneratedAssignment?> GetInProgressAsync(CancellationToken ct = default);
}
