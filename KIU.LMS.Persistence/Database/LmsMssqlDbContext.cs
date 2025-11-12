using Module = KIU.LMS.Domain.Entities.SQL.Module;

public sealed class LmsMssqlDbContext : DbContext
{
    public LmsMssqlDbContext(DbContextOptions<LmsMssqlDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Module> Modules { get; set; } = null!;
    public DbSet<SubModule> SubModules { get; set; } = null!;
    public DbSet<ModuleBank> ModuleBanks { get; set; }
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<Solution> Solutions { get; set; } = null!;
    public DbSet<UserCourse> UserCourses { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
    public DbSet<CourseMaterial> CourseMaterials { get; set; } = null!;
    public DbSet<CourseMeeting> CourseMeetings { get; set; } = null!;
    public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
    public DbSet<EmailQueue> EmailQueues { get; set; } = null!;
    public DbSet<QuestionBank> QuestionBanks { get; set; } = null!;
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<ExamResult> ExamResults { get; set; } = null!;
    public DbSet<Prompt> Prompts { get; set; } = null!;
    public DbSet<FileRecord> FileRecords { get; set; } = null!;
    public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}