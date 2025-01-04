namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IEmailProcessingService
{
    Task<string> ProcessTemplateAsync(string templateBody, string variables);
    Task<(string subject, string body)> PrepareEmailAsync(EmailTemplate template, string variables);
}
