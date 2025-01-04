namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record ResetPasswordCommand(string Password, string ConfirmPassword) : IRequest<Result>;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password);
    }
}

public class ResetPasswordCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _currentUser, IPasswordService _passwordService) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == _currentUser.UserId);

        if (user is null)
            return Result.Failure("Can't identify user");

        var passwordHash = _passwordService.HashPasword(request.Password, out byte[] salt);

        user.ChangePassword(passwordHash, salt, _currentUser.UserId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Password Changed Successfully");
    }
}