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
        await MigrateTableAsync(_mssql.Users, _pg.Users, "User");
        await MigrateTableAsync(_mssql.Courses, _pg.Courses,  "Course");
        await MigrateTableAsync(_mssql.Modules, _pg.Modules, "Module");
        await MigrateTableAsync(_mssql.ModuleBanks, _pg.ModuleBanks, "ModuleBank");
        await MigrateTableAsync(_mssql.SubModules, _pg.SubModules, "SubModule");
        await MigrateTableAsync(_mssql.Topics, _pg.Topics, "Topic");
        await MigrateTableAsync(_mssql.Assignments, _pg.Assignments,  "Assignment");
        await MigrateTableAsync(_mssql.Solutions, _pg.Solutions,  "Solution");
        await MigrateTableAsync(_mssql.UserCourses, _pg.UserCourses, "UserCourse");
        await MigrateTableAsync(_mssql.UserDevices, _pg.UserDevices, "UserDevice");
        await MigrateTableAsync(_mssql.CourseMaterials, _pg.CourseMaterials, "CourseMaterial");
        await MigrateTableAsync(_mssql.CourseMeetings, _pg.CourseMeetings, "CourseMeeting");
        await MigrateTableAsync(_mssql.EmailTemplates, _pg.EmailTemplates, "EmailTemplate");
        await MigrateTableAsync(_mssql.EmailQueues, _pg.EmailQueues, "EmailQueue");
        await MigrateTableAsync(_mssql.QuestionBanks, _pg.QuestionBanks, "QuestionBank");
        await MigrateTableAsync(_mssql.Quizzes, _pg.Quizzes, "Quiz");
        await MigrateTableAsync(_mssql.ExamResults, _pg.ExamResults, "ExamResult");
        await MigrateTableAsync(_mssql.Prompts, _pg.Prompts, "Prompt");
        await MigrateTableAsync(_mssql.FileRecords, _pg.FileRecords, "FileRecord");
        await MigrateTableAsync(_mssql.LoginAttempts, _pg.LoginAttempts, "LoginAttempt");
    }

    public async Task MigrateTableAsync<TEntity>(
        DbSet<TEntity> source,
        DbSet<TEntity> target,
        string tableName) where TEntity : class
    {    try
        {
            var data = await source.AsNoTracking().ToListAsync();
            if (!data.Any())
                return;

            if (tableName == "SubModule")
            {
                var existingModuleBankIds = await _pg.ModuleBanks
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToListAsync();

                data = data
                    .Where(e =>
                    {
                        var moduleBankId = (Guid?)e.GetType()
                            .GetProperty("ModuleBankId")?
                            .GetValue(e);

                        
                        return moduleBankId == null || existingModuleBankIds.Contains(moduleBankId.Value);
                    })
                    .ToList();

            }
            
            if (tableName == "Topic")
            {
                var existingCourseIds = await _pg.Courses
                    .AsNoTracking()
                    .Select(c => c.Id)
                    .ToListAsync();

                data = data
                    .Where(e =>
                    {
                        var courseId = (Guid?)e.GetType()
                            .GetProperty("CourseId")?
                            .GetValue(e);

                        return courseId != null && existingCourseIds.Contains(courseId.Value);
                    })
                    .ToList();

            }
            
           var existingIds = await target.AsNoTracking()
                .Select(e => EF.Property<Guid>(e, "Id"))
                .ToListAsync();

            var newData = data
                .Where(e => !existingIds.Contains((Guid)e.GetType().GetProperty("Id")!.GetValue(e)!))
                .ToList();
            
            if (!newData.Any())
               return;

            await target.AddRangeAsync(newData);
            await _pg.SaveChangesAsync();

            Console.WriteLine($"✅ {tableName} — added {newData.Count} new rows");
        }
        catch (Exception ex)
        {
            throw new Exception($"❌ table {tableName}: {ex.Message}", ex);
        }
    }

}