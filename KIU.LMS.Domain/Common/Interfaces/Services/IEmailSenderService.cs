namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IEmailSenderService
{
    Task SendEmailAsync(string to, string subject, string body);
}
