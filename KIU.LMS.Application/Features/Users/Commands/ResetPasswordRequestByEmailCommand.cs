namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record ResetPasswordRequestByEmailCommand(string Email) : IRequest<Result>;

public sealed class ResetPasswordRequestByEmailHandler(
    IUnitOfWork _unitOfWork, 
    IRedisRepository<string> _redis,
    ICurrentUserService _currentUser,
    FrontSettings _settings)  : IRequestHandler<ResetPasswordRequestByEmailCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordRequestByEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        
        if (user is null)
            return Result.Failure("User not found");
        
        var template = await _unitOfWork.EmailTemplateRepository.SingleOrDefaultAsync(x => x.Type == EmailType.PasswordReset);

        if (template is null)
            return Result.Failure("Can't find email template");
        
        var token = TextGenerator.GenerateRandomToken(16);

        var url = $"{_settings.Domain}/reset-password/{token}";
        
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