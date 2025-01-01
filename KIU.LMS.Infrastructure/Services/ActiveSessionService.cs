namespace KIU.LMS.Infrastructure.Services;

public class ActiveSessionService(IRedisRepository<DeviceInfo> _redisRepository, IHubContext<NotificationHub> _hubContext) : IActiveSessionService
{
    public async Task<bool> IsActiveSessionAsync(Guid userId, string deviceId)
    {
        return await _redisRepository.ExistsAsync($"user:{userId}:devices:{deviceId}");
    }

    public async Task SetActiveSessionAsync(Guid userId, string deviceId, DeviceInfo deviceInfo)
    {
        var existingDevices = await GetAllUserDevicesAsync(userId);

        foreach (var device in existingDevices)
        {
            if (device.Identifier != deviceId)
            {
                await RemoveActiveSessionAsync(userId, device.Identifier);
                await _hubContext.Clients
                    .Group(device.Identifier)
                    .SendAsync("SessionExpired", "სესია დასრულდა სხვა მოწყობილობიდან ავტორიზაციის გამო");
            }
        }

        await _redisRepository.SetAsync(
            $"user:{userId}:devices:{deviceId}",
            deviceInfo,
            TimeSpan.FromDays(30));
    }

    public async Task RemoveActiveSessionAsync(Guid userId, string deviceId)
    {
        await _redisRepository.DeleteAsync($"user:{userId}:devices:{deviceId}");
        await _hubContext.Clients
            .Group(deviceId)
            .SendAsync("SessionExpired", "თქვენი სესია დასრულდა");
    }

    public async Task<DeviceInfo?> GetActiveDeviceInfoAsync(Guid userId)
    {
        var devices = await GetAllUserDevicesAsync(userId);
        return devices.FirstOrDefault();
    }

    public async Task<IEnumerable<DeviceInfo>> GetAllUserDevicesAsync(Guid userId)
    {
        var pattern = $"user:{userId}:devices:*";
        var keys = await _redisRepository.GetKeysByPatternAsync(pattern);
        var devices = new List<DeviceInfo>();

        foreach (var key in keys)
        {
            var device = await _redisRepository.GetAsync(key);
            if (device != null)
            {
                devices.Add(device);
            }
        }

        return devices;
    }
}