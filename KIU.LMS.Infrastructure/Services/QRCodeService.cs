using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IRedisRepository<QrCodeEntry> _redis;
    private readonly ILogger<QrCodeService> _logger;
    private const string ApiBaseUrl = "http://learn-with-ai.kiu.edu.ge"; 
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(30);

    public QrCodeService(IRedisRepository<QrCodeEntry> redis, ILogger<QrCodeService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<(string qrUrl, DateTimeOffset expiresAt)> CreateQrAsync(Guid courseId)
    {
        var existingKeys = await _redis.GetKeysByPatternAsync($"{courseId}:*");
        var existingKey = existingKeys.FirstOrDefault();

        if (!string.IsNullOrEmpty(existingKey))
        {
            var existing = await _redis.GetAsync(existingKey.Replace("qr:", ""));
            if (existing != null && existing.ExpiresAt > DateTimeOffset.UtcNow)
            {
                var existingUrl = $"{ApiBaseUrl}/api/qrcode/{existing.Token}";
                return (existingUrl, existing.ExpiresAt);
            }
        }

        var token = GenerateToken();
        var redirectUrl = $"{ApiBaseUrl}/api/course-registration?courseId={courseId}";

        var entry = new QrCodeEntry(token, redirectUrl);

        await _redis.SetAsync($"{courseId}:{token}", entry, DefaultTtl);

        var qrUrl = $"{ApiBaseUrl}/api/qrcode/{token}";
        return (qrUrl, entry.ExpiresAt);
    }

    public async Task<string?> GetRedirectUrlAsync(string token)
    {
        var keys = await _redis.GetKeysByPatternAsync($"*:{token}");
        var key = keys.FirstOrDefault();
        if (key is null)
            return null;

        var entry = await _redis.GetAsync(key);
        if (entry is null || entry.ExpiresAt < DateTimeOffset.UtcNow)
            return null;

        return entry.RedirectUrl;
    }

    private static string GenerateToken()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}