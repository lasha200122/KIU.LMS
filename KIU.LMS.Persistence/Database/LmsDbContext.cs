namespace KIU.LMS.Persistence.Database;

public sealed class LmsDbContext(DbContextOptions<LmsDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CourseMaterial> CourseMaterials { get; set; } = null!;
    public DbSet<CourseMeeting> CourseMeetings { get; set; } = null!;
    public DbSet<EmailQueue> EmailQueues { get; set; } = null!;
    public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<ExamAttempt> ExamAttempts { get; set; } = null!;
    public DbSet<ExamConfiguration> ExamConfigurations { get; set; } = null!;
    public DbSet<ExamQuestion> ExamQuestions { get; set; } = null!;
    public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;
    public DbSet<QuestionBank> QuestionBanks { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserCourse> UserCourses { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Solution> Solutions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
