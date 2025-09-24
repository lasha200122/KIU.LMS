using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public class ModuleRepository(LmsDbContext _context) : BaseRepository<Domain.Entities.SQL.Module>(_context), IModuleRepository
{
    public async Task<IEnumerable<Domain.Entities.SQL.Module>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Modules
            .Include(m => m.ModuleBanks)
            .ThenInclude(m => m.SubModules)
            .Where(m => m.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<Domain.Entities.SQL.Module?> GetByIdAsync(Guid id)
    {
        return await _context.Modules
            .Include(m => m.Course)
            .Include(m => m.ModuleBanks)
            .ThenInclude(m => m.SubModules)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}


public class SubModuleRepository(LmsDbContext db) : BaseRepository<SubModule>(db), ISubModuleRepository
{
}

public class ModuleBankRepository(LmsDbContext db) : BaseRepository<ModuleBank>(db), IModuleBankRepository
{
}
