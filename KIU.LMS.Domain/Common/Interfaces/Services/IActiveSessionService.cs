namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IActiveSessionService
{
    Task<bool> IsActiveSessionAsync(Guid userId, string deviceId);
    Task SetActiveSessionAsync(Guid userId, string deviceId, DeviceInfo deviceInfo);
    Task RemoveActiveSessionAsync(Guid userId, string deviceId);
    Task<DeviceInfo?> GetActiveDeviceInfoAsync(Guid userId);
    Task<IEnumerable<DeviceInfo>> GetAllUserDevicesAsync(Guid userId);
}