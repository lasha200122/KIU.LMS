namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

public interface IModuleRepository : IBaseRepository<Module>
{
    Task<IEnumerable<Module>> GetByCourseIdAsync(Guid courseId);
    Task<Module?> GetByIdAsync(Guid id);
}


public interface IModuleBankRepository
    : IBaseRepository<ModuleBank> { }
