namespace KIU.LMS.Domain.Common.Enums.Email;

public enum EmailStatus
{
    None = 0,
    Pending = 1,
    Processing = 2,
    Sent = 3,
    Failed = 4,
    Retry = 5,
    Cancelled = 6
}
