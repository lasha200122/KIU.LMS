using Microsoft.AspNetCore.Http;

namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record RegisterUsersCommand(IFormFile File) : IRequest<Result>;


public class RegisterUsersCommandHandler(
    IUnitOfWork _unitOfWork,
    IPasswordService _password,
    IRedisRepository<string> _redis,
    ICurrentUserService _currentUser,
    IExcelProcessor _excel,
    FrontSettings _settings) : IRequestHandler<RegisterUsersCommand, Result>
{
    public async Task<Result> Handle(RegisterUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await _excel.ProcessStudentsExcelFile(request.File);

        if (!users.IsValid)
            return Result.Failure("Invalid excel format");

        var emailTemplate = await _unitOfWork.EmailTemplateRepository.SingleOrDefaultAsync(x => x.Type == EmailType.WelcomeMessage);

        if (emailTemplate is null)
            return Result.Failure("Email Template Can't be found");

        var existedUsers = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);

        var emails = existedUsers.Select(x => x.Email.ToLower()).ToHashSet();

        foreach (var user in users.ValidStudents) 
        {
            if (emails.Contains(user.Email))
                continue;

            var password = TextGenerator.GenerateRandomPassword(8);
            var token = TextGenerator.GenerateRandomToken(16);

            var passwordHash = _password.HashPasword(password, out byte[] salt);

            string url = $"{_settings.Domain}/reset-password/{token}";

            var newUser = new User(
                Guid.NewGuid(),
                user.FirstName.Trim().ToLower(),
                user.LastName.Trim().ToLower(),
                user.Email,
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


            await _redis.SetAsync(token, newUser.Id.ToString(), TimeSpan.FromDays(1));
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Users Registered Successfully");
    }
}