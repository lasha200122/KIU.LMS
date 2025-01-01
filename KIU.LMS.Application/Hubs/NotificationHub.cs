namespace KIU.LMS.Application.Hubs;

public class NotificationHub(ICurrentUserService _currentUserService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (!string.IsNullOrEmpty(_currentUserService.DeviceId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, _currentUserService.DeviceId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (!string.IsNullOrEmpty(_currentUserService.DeviceId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _currentUserService.DeviceId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}