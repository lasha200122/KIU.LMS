namespace KIU.LMS.Domain.Entities.SQL;

public class UserDevice : Aggregate
{
    public Guid UserId { get; private set; }
    public string DeviceIdentifier { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;

    public UserDevice() { }

    public UserDevice(
        Guid id,
        Guid userId,
        string deviceIdentifier,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        UserId = userId;
        DeviceIdentifier = deviceIdentifier;
        Validate(this);
    }

    private void Validate(UserDevice device)
    {
        if (device.UserId == default)
            throw new Exception("მომხმარებლის ID სავალდებულოა");
        if (string.IsNullOrEmpty(device.DeviceIdentifier))
            throw new Exception("მოწყობილობის იდენტიფიკატორი სავალდებულოა");
    }
}