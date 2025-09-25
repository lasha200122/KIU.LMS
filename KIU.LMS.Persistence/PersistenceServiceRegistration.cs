using KIU.LMS.Persistence.Database.Services;
using Microsoft.AspNetCore.Builder;
using StackExchange.Redis;

namespace KIU.LMS.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();

        var mongodbOptions = configuration.GetSection(nameof(MongodbSettings)).Get<MongodbSettings>();
        services.AddSingleton(mongodbOptions!);
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        var redisOptions = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>()!;
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisOptions.ConnectionString));
        services.AddScoped(typeof(IRedisRepository<>), typeof(RedisRepository<>));

        services.AddDbContext<LmsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<LmsMssqlDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MssqlConnection")));

        services.AddScoped<DataMigrationService>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
        services.AddScoped<ICourseMeetingRepository, CourseMeetingRepository>();
        services.AddScoped<IEmailQueueRepository, EmailQueueRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
        services.AddScoped<IQuestionBankRepository, QuestionBankRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserCourseRepository, UserCourseRepository>();
        services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>(); 
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISolutionRepository, SolutionRepository>();
        services.AddScoped<IPromptRepository, PromptRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuizBankRepository, QuizBankRepository>();
        services.AddScoped<IExamResultRepository, ExamResultRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ISubModuleRepository, SubModuleRepository>();
        services.AddScoped<IModuleBankRepository, ModuleBankRepository>();
        services.AddScoped<IFileRepository, FileRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        logger.Information("Layer loaded: {Layer} ", thisAssembly.GetName().Name);

        return services;
    }

    public static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app, Serilog.ILogger logger)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();

                // Check if there are any pending migrations
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    logger.Information("Found {Count} pending migration(s). Applying...", pendingMigrations.Count());

                    foreach (var migration in pendingMigrations)
                    {
                        logger.Information("Pending migration: {MigrationName}", migration);
                    }

                    // Apply pending migrations
                    await dbContext.Database.MigrateAsync();

                    logger.Information("All pending migrations applied successfully");
                }
                else
                {
                    logger.Information("Database is up to date. No pending migrations");
                }

                // Verify connection
                var canConnect = await dbContext.Database.CanConnectAsync();
                if (canConnect)
                {
                    logger.Information("Database connection verified successfully");
                }
                else
                {
                    logger.Error("Unable to connect to the database");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while applying migrations");
                throw; // Re-throw to stop application startup if migrations fail
            }
        }

        return app;
    }
}
