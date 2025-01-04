namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record VerifyEmailCommand(string Token, string Password, string ConfirmPassword) : IRequest<Result>;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand> 
{
    public VerifyEmailCommandValidator() 
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .Length(16);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password);
    }
}

public class VerifyEmailCommandHandler(
    IUnitOfWork _unitOfWork,
    IRedisRepository<string> _redis,
    IPasswordService _password,
    ICurrentUserService _currentUser) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var session = await _redis.GetAsync(request.Token);

        if (string.IsNullOrEmpty(session))
            return Result.Failure("Invalid Token");

        var userId = new Guid(session);

        var user = await _unitOfWork.UserRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == userId);

        if (user is null)
            return Result.Failure("Cant find user");

        var passwordHash = _password.HashPasword(request.Password, out byte[] salt);

        user.ChangePassword(passwordHash, salt, _currentUser.UserId);
        user.VerifyEmail();

        await _unitOfWork.SaveChangesAsync();

        await _redis.DeleteAsync(request.Token);

        return Result.Success("User Verification was successfull");
    }
}