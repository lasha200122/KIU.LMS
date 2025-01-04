namespace KIU.LMS.Application.Features.Auth.Commands;

public sealed record LogoutCommand : IRequest<Result>;

public class LogoutCommandHandler(
    ICurrentUserService _currentUserService,
    IActiveSessionService _sessionService) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var deviceId = _currentUserService.DeviceId;

        if (string.IsNullOrEmpty(deviceId))
            return Result.Failure("Invalid Device ID");

        var isActiveSession = await _sessionService.IsActiveSessionAsync(userId, deviceId);
        if (!isActiveSession)
            return Result.Failure("Session not found");

        await _sessionService.RemoveActiveSessionAsync(userId, deviceId);

        return Result.Success();
    }
}