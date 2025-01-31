namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    ICourseMaterialRepository CourseMaterialRepository { get; }
    ICourseMeetingRepository CourseMeetingRepository { get; }
    IEmailQueueRepository EmailQueueRepository { get; }
    IEmailTemplateRepository EmailTemplateRepository { get; }
    IExamRepository ExamRepository { get; }
    IExamAttemptRepository ExamAttemptRepository { get; }
    IExamConfigurationRepository ExamConfigurationRepository { get; }
    IExamQuestionRepository ExamQuestionRepository { get; }
    ILoginAttemptRepository LoginAttemptRepository { get; }
    IQuestionBankRepository QuestionBankRepository { get; }
    IUserRepository UserRepository { get; }
    IUserCourseRepository UserCourseRepository { get; }
    IUserDeviceRepository UserDeviceRepository { get; }
    ITopicRepository TopicRepository { get; }
    IAssignmentRepository AssignmentRepository { get; }

    Task CreateTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackAsync();
    Task SaveChangesAsync();
}
