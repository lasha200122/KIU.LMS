namespace KIU.LMS.Domain.Entities.SQL;

public class LoginAttempt : Aggregate
{
    public Guid UserId { get; private set; }
    public string IpAddress { get; private set; } = null!;
    public string UserAgent { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string DeviceIdentifier { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    public LoginAttempt() { }

    public LoginAttempt(
        Guid id,
        Guid userId,
        string ipAddress,
        string userAgent,
        string status,
        string deviceIdentifier,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Status = status;
        DeviceIdentifier = deviceIdentifier;
        Validate(this);
    }

    private void Validate(LoginAttempt attempt)
    {
        if (attempt.UserId == default)
            throw new Exception("მომხმარებლის ID სავალდებულოა");
        if (string.IsNullOrEmpty(attempt.IpAddress))
            throw new Exception("IP მისამართი სავალდებულოა");
        if (string.IsNullOrEmpty(attempt.DeviceIdentifier))
            throw new Exception("მოწყობილობის იდენტიფიკატორი სავალდებულოა");
    }
}