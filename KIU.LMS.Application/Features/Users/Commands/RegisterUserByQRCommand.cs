namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record RegisterUserByQRCommand(
    Guid CourseId,
    string FirstName,
    string LastName,
    string Email,
    string School) : IRequest<Result>;

public sealed class RegisterUserByQRCommandValidator : AbstractValidator<RegisterUserByQRCommand> 
{
    public RegisterUserByQRCommandValidator() 
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.FirstName)
            .NotNull();

        RuleFor(x => x.LastName)
            .NotNull();

        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.School)
            .NotNull();
    }
}


public sealed class RegisterUserByQRCommandHandler(
    IUnitOfWork _unitOfWork,
    IPasswordService _password,
    IRedisRepository<string> _redis,
    FrontSettings _settings) : IRequestHandler<RegisterUserByQRCommand, Result>
{
    public async Task<Result> Handle(RegisterUserByQRCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await _unitOfWork.CourseRepository.ExistsAsync(x => x.Id == request.CourseId);

        if (!courseExists)
            return Result.Failure("Can't find a course");

        var emailToLower = request.Email.Trim().ToLower();

        var userExist = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Email == emailToLower);

        if (userExist != null) 
        {
            var courseHaveUser = await _unitOfWork.UserCourseRepository.ExistsAsync(x => x.CourseId == request.CourseId && x.UserId == userExist.Id);

            if (courseHaveUser)
                return Result.Failure("Course have user");

            var add = new UserCourse(
                Guid.NewGuid(),
                userExist.Id,
                request.CourseId,
                DateTimeOffset.UtcNow.AddYears(1),
                userExist.Id);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

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
            request.School,
            Guid.NewGuid());

        var email = new EmailQueue(
            Guid.NewGuid(),
            emailTemplate.Id,
            newUser.Email,
            EmailTemplateUtils.RegisterUserVariableBuilder(newUser, url),
            newUser.Id);

        var userCourse = new UserCourse(
            Guid.NewGuid(),
            newUser.Id,
            request.CourseId,
            DateTimeOffset.UtcNow.AddYears(1),
            newUser.Id);

        await _unitOfWork.UserRepository.AddAsync(newUser);
        await _unitOfWork.EmailQueueRepository.AddAsync(email);
        await _unitOfWork.UserCourseRepository.AddAsync(userCourse);

        await _unitOfWork.SaveChangesAsync();

        await _redis.SetAsync(token, newUser.Id.ToString(), TimeSpan.FromDays(1));

        return Result.Success("User Registered Successfully");

    }
}
