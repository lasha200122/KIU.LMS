namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IGeneratedQuestionRepository : IBaseRepository<GeneratedQuestion>
{
    Task AddSafetyAsync(GeneratedQuestion q, int maxAllowed, Guid generatedAssigmentId);
}
