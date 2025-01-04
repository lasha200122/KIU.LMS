namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record RegisterUserCommand(string FirstName, string LastName, string Email) : IRequest<Result>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand> 
{
    public RegisterUserCommandValidator() 
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

public class RegisterUserCommandHandler(
    IUnitOfWork _unitOfWork,
    IPasswordService _password,
    IRedisRepository<string> _redis,
    ICurrentUserService _currentUser,
    FrontSettings _settings) : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailToLower = request.Email.Trim().ToLower();

        var userExist = await _unitOfWork.UserRepository.ExistsAsync(x => x.Email == emailToLower, cancellationToken);

        if (userExist)
            return Result.Failure("User is already registered");

        var emailTemplate = await _unitOfWork.EmailTemplateRepository.SingleOrDefaultAsync(x => x.Type == EmailType.WelcomeMessage);

        if (emailTemplate is null)
            return Result.Failure("Email Template Can't be found");

        var password = TextGenerator.GenerateRandomPassword(8);
        var token = TextGenerator.GenerateRandomToken(16);

        var passwordHash = _password.HashPasword(password, out byte[] salt);

        string url = $"{_settings.Domain}/verify/{token}";

        var newUser = new User(
            Guid.NewGuid(),
            request.FirstName.Trim().ToLower(),
            request.LastName.Trim().ToLower(),
            emailToLower,
            UserRole.Student,
            passwordHash,
            Convert.ToHexString(salt),
            _currentUser.UserId);

        var email = new EmailQueue(
            Guid.NewGuid(),
            emailTemplate.Id,
            newUser.Email,
            EmailTemplateUtils.RegisterUserVariableBuilder(newUser, url),
            _currentUser.UserId);

        await _unitOfWork.UserRepository.AddAsync(newUser);
        await _unitOfWork.EmailQueueRepository.AddAsync(email);

        await _unitOfWork.SaveChangesAsync();

        await _redis.SetAsync(token, newUser.Id.ToString(), TimeSpan.FromDays(1));

        return Result.Success("User Registered Successfully");
    }
}