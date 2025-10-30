namespace KIU.LMS.Domain.Entities.NoSQL;

public class QrCodeEntry
{
    public string Token { get; set; } = null!;
    public string RedirectUrl { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; private set; } = DateTimeOffset.UtcNow.AddMinutes(30);

    public QrCodeEntry() { }
    
    public  QrCodeEntry(string token, string redirectUrl)
    {
        Token = token;
        RedirectUrl = redirectUrl;
    }
}
