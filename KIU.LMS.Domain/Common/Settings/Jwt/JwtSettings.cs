namespace KIU.LMS.Domain.Common.Settings.Jwt;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public double ExpiryMinutes { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int RefreshTokenValidityInDays { get; set; }
}
