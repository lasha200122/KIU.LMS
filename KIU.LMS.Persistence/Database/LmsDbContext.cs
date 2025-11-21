using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KIU.LMS.Persistence.Database;

public sealed class LmsDbContext(DbContextOptions<LmsDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CourseMaterial> CourseMaterials { get; set; } = null!;
    public DbSet<CourseMeeting> CourseMeetings { get; set; } = null!;
    public DbSet<EmailQueue> EmailQueues { get; set; } = null!;
    public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
    public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;
    public DbSet<QuestionBank> QuestionBanks { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserCourse> UserCourses { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Solution> Solutions { get; set; } = null!;
    public DbSet<Prompt> Prompts { get; set; } = null!;
    public DbSet<ExamResult> ExamResults { get; set; } = null!;
    public DbSet<Domain.Entities.SQL.Module> Modules { get; set; } = null!;
    public DbSet<SubModule> SubModules { get; set; } = null!;
    public DbSet<ModuleBank> ModuleBanks { get; set; }
    public DbSet<FileRecord> FileRecords { get; set; } = null!;
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<AssignmentSolutionJob> AssignmentSolutionJobs { get; set; } = null!;
    public DbSet<GeneratedAssignment> GeneratedAssignments { get; set; } = null!;
    public DbSet<GeneratedQuestion> GeneratedQuestions { get; set; } = null!;
    public DbSet<VotingSession> VotingSessions { get; set; } = null!;
    public DbSet<VotingOption> VotingOptions { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;


    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
        base.OnModelCreating(builder);
        
        var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, DateTimeOffset>(
            v => v.ToUniversalTime(),          
            v => DateTime.SpecifyKind(v.DateTime, DateTimeKind.Utc));

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset) ||
                    property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetValueConverter(dateTimeOffsetConverter);
                }
            }
        }

    }
}
