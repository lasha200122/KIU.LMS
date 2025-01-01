namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    string FullName { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
    string? DeviceId { get; }
}