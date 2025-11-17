using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataMigration;

public partial class KiuLmsContext : DbContext
{
    public KiuLmsContext()
    {
    }

    public KiuLmsContext(DbContextOptions<KiuLmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }

    public virtual DbSet<CourseMeeting> CourseMeetings { get; set; }

    public virtual DbSet<EmailQueue> EmailQueues { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<ExamResult> ExamResults { get; set; }

    public virtual DbSet<FileRecord> FileRecords { get; set; }

    public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModuleBank> ModuleBanks { get; set; }

    public virtual DbSet<Prompt> Prompts { get; set; }

    public virtual DbSet<QuestionBank> QuestionBanks { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizBank> QuizBanks { get; set; }

    public virtual DbSet<Solution> Solutions { get; set; }

    public virtual DbSet<SubModule> SubModules { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCourse> UserCourses { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Aigrader).HasColumnName("AIGrader");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Score).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Course).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Prompt).WithMany(p => p.Assignments).HasForeignKey(d => d.PromptId);

            entity.HasOne(d => d.Topic).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.ToTable("CourseMaterial");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseMaterials).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.CourseMaterialParent).WithMany(p => p.InverseCourseMaterialParent).HasForeignKey(d => d.CourseMaterialParentId);
        });

        modelBuilder.Entity<CourseMeeting>(entity =>
        {
            entity.ToTable("CourseMeeting");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseMeetings).HasForeignKey(d => d.CourseId);
        });

        modelBuilder.Entity<EmailQueue>(entity =>
        {
            entity.ToTable("EmailQueue");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.ToEmail).HasMaxLength(256);

            entity.HasOne(d => d.Template).WithMany(p => p.EmailQueues).HasForeignKey(d => d.TemplateId);
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable("EmailTemplate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Subject).HasMaxLength(200);
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.ToTable("ExamResult");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Score).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SessionId).HasDefaultValue("");

            entity.HasOne(d => d.Quiz).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FileRecord>(entity =>
        {
            entity.ToTable("FileRecord");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentType).HasMaxLength(150);
            entity.Property(e => e.FileName).HasMaxLength(300);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ObjectId).HasMaxLength(500);
            entity.Property(e => e.ObjectType).HasMaxLength(200);
        });

        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.ToTable("LoginAttempt");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DeviceIdentifier).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.LoginAttempts).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Module");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ModuleBank>(entity =>
        {
            entity.ToTable("ModuleBank");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Module).WithMany(p => p.ModuleBanks)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModuleBank_Module");
        });

        modelBuilder.Entity<Prompt>(entity =>
        {
            entity.ToTable("Prompt");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.ToTable("QuestionBank");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Module).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionBank_Module");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.ToTable("Quiz");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MinusScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Score).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Course).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Topic).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<QuizBank>(entity =>
        {
            entity.ToTable("QuizBank");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.QuestionBank).WithMany(p => p.QuizBanks)
                .HasForeignKey(d => d.QuestionBankId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizBanks)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Solution>(entity =>
        {
            entity.ToTable("Solution");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Assignment).WithMany(p => p.Solutions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Solutions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SubModule>(entity =>
        {
            entity.ToTable("SubModule");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodeGenerationPrompt).HasMaxLength(2000);
            entity.Property(e => e.CodeGraidingPrompt).HasMaxLength(2000);
            entity.Property(e => e.CodeSolution).HasMaxLength(4000);
            entity.Property(e => e.Solution).HasMaxLength(2000);
            entity.Property(e => e.TaskDescription).HasMaxLength(2000);

            entity.HasOne(d => d.ModuleBank).WithMany(p => p.SubModules)
                .HasForeignKey(d => d.ModuleBankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubModule_ModuleBank");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topic");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Course).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Institution).HasMaxLength(400);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PasswordSalt).HasMaxLength(500);
        });

        modelBuilder.Entity<UserCourse>(entity =>
        {
            entity.ToTable("UserCourse");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Course).WithMany(p => p.UserCourses).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.User).WithMany(p => p.UserCourses).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.ToTable("UserDevice");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DeviceIdentifier).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
