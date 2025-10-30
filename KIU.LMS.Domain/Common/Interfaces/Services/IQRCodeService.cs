namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IQrCodeService
{
    Task<(string qrUrl, DateTimeOffset expiresAt)> CreateQrAsync(Guid courseId);
    Task<string?> GetRedirectUrlAsync(string token);
}