namespace KIU.LMS.Application.Workers;

public class EmailQueueWorker(IServiceProvider _serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var emailProcessor = scope.ServiceProvider.GetRequiredService<IEmailProcessingService>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var pendingEmailsQueue = await unitOfWork.EmailQueueRepository.GetWhereAsTrackingIncludedAsync(x => x.Status == EmailStatus.Pending ||
                               (x.Status == EmailStatus.Retry && x.RetryCount < 3), x => x.Template);

                var pendingEmails = pendingEmailsQueue.Take(100).ToList();

                foreach (var queueItem in pendingEmails)
                {
                    try
                    {
                        queueItem.UpdateStatus(EmailStatus.Processing);

                        await unitOfWork.SaveChangesAsync();

                        var (subject, body) = await emailProcessor.PrepareEmailAsync(
                            queueItem.Template,
                            queueItem.Variables);

                        await emailSender.SendEmailAsync(
                            queueItem.ToEmail,
                            subject,
                            body);

                        queueItem.UpdateStatus(EmailStatus.Sent);
                    }
                    catch (Exception ex)
                    {
                        if (queueItem.RetryCount < 3)
                        {
                            queueItem.IncrementRetryCount();
                            queueItem.UpdateStatus(EmailStatus.Retry, ex.Message);
                        }
                        else
                        {
                            queueItem.UpdateStatus(EmailStatus.Failed, ex.Message);
                        }
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}