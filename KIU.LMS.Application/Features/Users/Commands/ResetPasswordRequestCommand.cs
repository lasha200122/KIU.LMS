namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record ResetPasswordRequestCommand(Guid UserId) : IRequest<Result>;

public class ResetPasswordRequestCommandValidator : AbstractValidator<ResetPasswordRequestCommand> 
{
    public ResetPasswordRequestCommandValidator() 
    {
        RuleFor(x => x.UserId)
            .NotNull();
    }
}

public class ResetPasswordRequestCommandHandler(IUnitOfWork _unitOfWork, IRedisRepository<string> _redis, ICurrentUserService _currentUser, FrontSettings _settings) : IRequestHandler<ResetPasswordRequestCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Id == request.UserId);

        if (user is null)
            return Result.Failure("Can't find user");

        var template = await _unitOfWork.EmailTemplateRepository.SingleOrDefaultAsync(x => x.Type == EmailType.PasswordReset);

        if (template is null)
            return Result.Failure("Can't find email template");

        var token = TextGenerator.GenerateRandomToken(16);

        string url = $"{_settings.Domain}/verify/{token}";

        var variables = EmailTemplateUtils.ResetPasswordVariableBuilder(user, url);

        var email = new EmailQueue(
            Guid.NewGuid(),
            template.Id,
            user.Email,
            variables,
            _currentUser.UserId);

        await _unitOfWork.EmailQueueRepository.AddAsync(email);

        await _redis.SetAsync(token, user.Id.ToString());

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Password Reset Request Sent successfully");
    }
}