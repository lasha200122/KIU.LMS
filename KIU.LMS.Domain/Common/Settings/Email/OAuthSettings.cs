namespace KIU.LMS.Domain.Common.Settings.Email;

public class OAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = new string[0];
    public string FromEmail { get; set; } = string.Empty;
}