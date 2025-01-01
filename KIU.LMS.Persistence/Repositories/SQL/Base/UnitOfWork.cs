namespace KIU.LMS.Persistence.Repositories.SQL.Base;

public class UnitOfWork(LmsDbContext _dbContext) : IUnitOfWork
{
    public ICourseRepository CourseRepository => new CourseRepository(_dbContext);
    public ICourseMaterialRepository CourseMaterialRepository => new CourseMaterialRepository(_dbContext);
    public ICourseMeetingRepository CourseMeetingRepository => new CourseMeetingRepository(_dbContext);
    public IEmailQueueRepository EmailQueueRepository => new EmailQueueRepository(_dbContext);
    public IEmailTemplateRepository EmailTemplateRepository => new EmailTemplateRepository(_dbContext);
    public IExamRepository ExamRepository => new ExamRepository(_dbContext);
    public IExamAttemptRepository ExamAttemptRepository => new ExamAttemptRepository(_dbContext);
    public IExamConfigurationRepository ExamConfigurationRepository => new ExamConfigurationRepository(_dbContext);
    public IExamQuestionRepository ExamQuestionRepository => new ExamQuestionRepository(_dbContext);
    public ILoginAttemptRepository LoginAttemptRepository => new LoginAttemptRepository(_dbContext);
    public IQuestionBankRepository QuestionBankRepository => new QuestionBankRepository(_dbContext);
    public IUserRepository UserRepository => new UserRepository(_dbContext);
    public IUserCourseRepository UserCourseRepository => new UserCourseRepository(_dbContext);
    public IUserDeviceRepository UserDeviceRepository => new UserDeviceRepository(_dbContext);

    public async Task CreateTransactionAsync() => await _dbContext.Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync() => await _dbContext.Database.CommitTransactionAsync();
    public async Task RollbackAsync() => await _dbContext.Database.RollbackTransactionAsync();
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}
