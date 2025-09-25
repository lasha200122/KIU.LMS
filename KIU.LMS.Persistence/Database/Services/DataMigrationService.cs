namespace KIU.LMS.Persistence.Database.Services;

public class DataMigrationService
{
    private readonly LmsMssqlDbContext _mssql;
    private readonly LmsDbContext _pg;

    public DataMigrationService(LmsMssqlDbContext mssql, LmsDbContext pg)
    {
        _mssql = mssql;
        _pg = pg;
    }

    public async Task MigrateAllAsync()
    {
        await MigrateTableAsync(_mssql.Users, _pg.Users);
        await MigrateTableAsync(_mssql.Courses, _pg.Courses);
        await MigrateTableAsync(_mssql.Modules, _pg.Modules);
        await MigrateTableAsync(_mssql.SubModules, _pg.SubModules);
        await MigrateTableAsync(_mssql.Topics, _pg.Topics);
        await MigrateTableAsync(_mssql.Assignments, _pg.Assignments);
        await MigrateTableAsync(_mssql.Solutions, _pg.Solutions);
        await MigrateTableAsync(_mssql.UserCourses, _pg.UserCourses);
        await MigrateTableAsync(_mssql.UserDevices, _pg.UserDevices);
        await MigrateTableAsync(_mssql.CourseMaterials, _pg.CourseMaterials);
        await MigrateTableAsync(_mssql.CourseMeetings, _pg.CourseMeetings);
        await MigrateTableAsync(_mssql.EmailTemplates, _pg.EmailTemplates);
        await MigrateTableAsync(_mssql.EmailQueues, _pg.EmailQueues);
        await MigrateTableAsync(_mssql.QuestionBanks, _pg.QuestionBanks);
        await MigrateTableAsync(_mssql.Quizzes, _pg.Quizzes);
        await MigrateTableAsync(_mssql.ExamResults, _pg.ExamResults);
        await MigrateTableAsync(_mssql.Prompts, _pg.Prompts);
        await MigrateTableAsync(_mssql.FileRecords, _pg.FileRecords);
        await MigrateTableAsync(_mssql.LoginAttempts, _pg.LoginAttempts);
    }
    public async Task MigrateTableAsync<TEntity>(DbSet<TEntity> source, DbSet<TEntity> target) where TEntity : class
    {
        try
        {
            var data = await source.AsNoTracking().ToListAsync();
            if (data.Count == 0) return;

            await target.AddRangeAsync(data);
            await _pg.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error migrating {typeof(TEntity).Name}: {ex.Message}", ex);
        }
    }
}