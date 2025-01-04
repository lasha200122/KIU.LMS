using System.Security.Claims;

namespace KIU.LMS.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken) : IRequest<Result<LoginCommandResponse>>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenCommandHandler(
    IUnitOfWork _unitOfWork,
    IJwtService _jwtService,
    ICurrentUserService _currentUserService,
    IActiveSessionService _sessionService) : IRequestHandler<RefreshTokenCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Result<LoginCommandResponse>.Failure("Invalid token");

        var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            throw new InvalidOperationException("User ID not found in token"));

        var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            return Result<LoginCommandResponse>.Failure("User not found");

        var deviceId = _currentUserService.DeviceId;
        if (string.IsNullOrEmpty(deviceId))
            return Result<LoginCommandResponse>.Failure("Invalid Device ID");

        var isActiveSession = await _sessionService.IsActiveSessionAsync(userId, deviceId);
        if (!isActiveSession)
            return Result<LoginCommandResponse>.Failure("Session expired");

        var newAccessToken = _jwtService.GenerateJwtToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var deviceInfo = new DeviceInfo(
            Identifier: deviceId,
            Device: deviceId,
            Browser: deviceId);

        await _sessionService.SetActiveSessionAsync(userId, deviceId, deviceInfo);

        var userMeta = new UserDto(
            $"{user.FirstName} {user.LastName}",
            user.Email.ToLower(),
            EnumTranslator.UserRoles(user.Role));

        return new LoginCommandResponse(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken.Token,
            User: userMeta);
    }
}