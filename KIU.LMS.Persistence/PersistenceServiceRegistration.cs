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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseMaterialRepository, CourseMaterialRepository>();
        services.AddScoped<ICourseMeetingRepository, CourseMeetingRepository>();
        services.AddScoped<IEmailQueueRepository, EmailQueueRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IExamAttemptRepository, ExamAttemptRepository>();
        services.AddScoped<IExamConfigurationRepository, ExamConfigurationRepository>();
        services.AddScoped<IExamQuestionRepository, ExamQuestionRepository>();
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

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        logger.Information("Layer loaded: {Layer} ", thisAssembly.GetName().Name);

        return services;
    }
}
