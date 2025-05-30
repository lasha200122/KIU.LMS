namespace KIU.LMS.Persistence.Repositories.SQL;

public class ModuleRepository(LmsDbContext db) : BaseRepository<Domain.Entities.SQL.Module>(db), IModuleRepository
{
}


public class SubModuleRepository(LmsDbContext db) : BaseRepository<SubModule>(db), ISubModuleRepository
{
}

public class ModuleBankRepository(LmsDbContext db) : BaseRepository<ModuleBank>(db), IModuleBankRepository
{
}
