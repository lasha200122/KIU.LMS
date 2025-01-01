namespace KIU.LMS.Application.Features.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginCommandResponse>>;

public sealed record LoginCommandResponse(
    string AccessToken,
    string RefreshToken,
    UserDto User);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public class LoginCommandHandler(
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService,
    ICurrentUserService _currentUserService,
    IJwtService _jwtService,
    IActiveSessionService _sessionService) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailToLower = request.Email.Trim().ToLower();

        var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Email == emailToLower);

        if (user is null)
            return Result<LoginCommandResponse>.Failure("Invalid Email or Password");

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash, Convert.FromHexString(user.PasswordSalt)))
            return Result<LoginCommandResponse>.Failure("Invalid Email or Password");

        var deviceId = _currentUserService.DeviceId;

        if (string.IsNullOrEmpty(deviceId))
            return Result<LoginCommandResponse>.Failure("Invalid Device ID");

        var token = _jwtService.GenerateJwtToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var userDevice = await _unitOfWork.UserDeviceRepository.SingleOrDefaultAsync(x => x.UserId == user.Id && x.DeviceIdentifier == deviceId);

        if (userDevice is null)
        {
            userDevice = new UserDevice(
                id: Guid.NewGuid(),
                userId: user.Id,
                deviceIdentifier: deviceId,
                createUserId: user.Id);

            await _unitOfWork.UserDeviceRepository.AddAsync(userDevice);
        }

        var deviceInfo = new DeviceInfo(
            Identifier: deviceId,
            Device: deviceId,
            Browser: deviceId);

        await _sessionService.SetActiveSessionAsync(user.Id, deviceId, deviceInfo);

        await _unitOfWork.SaveChangesAsync();

        var userMeta = new UserDto(
            $"{user.FirstName} {user.LastName}",
            user.Email.ToLower(),
            nameof(user.Role));

        return new LoginCommandResponse(
            AccessToken: token,
            RefreshToken: refreshToken.Token,
            User: userMeta);
    }
}
