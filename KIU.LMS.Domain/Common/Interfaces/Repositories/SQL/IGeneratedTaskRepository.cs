using KIU.LMS.Domain.Entities.SQL;

namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IGeneratedTaskRepository : IBaseRepository<GeneratedTask>
{
    Task<GeneratedAssignment?> GetTaskInProgressAsync(CancellationToken ct = default);
    Task AddSafetyAsync(GeneratedTask task, int maxCount, Guid assignmentId, CancellationToken ct = default);
}
