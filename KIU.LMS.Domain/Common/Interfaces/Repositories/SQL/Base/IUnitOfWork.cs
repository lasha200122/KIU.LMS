namespace KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    ICourseMaterialRepository CourseMaterialRepository { get; }
    ICourseMeetingRepository CourseMeetingRepository { get; }
    IEmailQueueRepository EmailQueueRepository { get; }
    IEmailTemplateRepository EmailTemplateRepository { get; }
    ILoginAttemptRepository LoginAttemptRepository { get; }
    IQuestionBankRepository QuestionBankRepository { get; }
    IUserRepository UserRepository { get; }
    IUserCourseRepository UserCourseRepository { get; }
    IUserDeviceRepository UserDeviceRepository { get; }
    ITopicRepository TopicRepository { get; }
    IAssignmentRepository AssignmentRepository { get; }
    ISolutionRepository SolutionRepository { get; }
    IPromptRepository PromptRepository { get; }
    IQuizRepository QuizRepository { get; }
    IQuizBankRepository QuizBankRepository { get; }
    IExamResultRepository ExamResultRepository { get; }
    IModuleRepository ModuleRepository { get; }
    ISubModuleRepository SubModuleRepository { get; }
    IModuleBankRepository ModuleBankRepository { get; }
    IAssignmentSolutionJobRepository AssignmentSolutionJobRepository { get; }
    Task CreateTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackAsync();
    Task SaveChangesAsync();
    IGeneratedAssignmentRepository GeneratedAssignmentRepository { get;  }
    IGeneratedQuestionRepository GeneratedQuestionRepository { get; }
}
