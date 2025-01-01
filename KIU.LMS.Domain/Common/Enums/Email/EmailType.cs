namespace KIU.LMS.Domain.Common.Enums.Email;

public enum EmailType
{
    None = 0,
    EmailVerification, 
    PasswordReset,            
    LoginNotification,       

    CourseEnrollment,          
    CourseAccess,             
    CourseAccessExpiring,

    ExamScheduled,
    ExamReminder,
    ExamResults,

    NewMaterialAvailable,

    WelcomeMessage,
    SystemMaintenance,
    AccountBlocked,
}
