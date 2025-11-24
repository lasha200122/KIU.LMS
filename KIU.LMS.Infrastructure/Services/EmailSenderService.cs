namespace KIU.LMS.Infrastructure.Services;

public class EmailSenderService(EmailSettings _settings) : IEmailSenderService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.Port);
            smtpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}