using Microsoft.AspNetCore.Http;

namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record CourseStudentsAddCommand(Guid Id, IFormFile File) : IRequest<Result>;

public class CourseStudentsAddCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current, IExcelProcessor _excel) : IRequestHandler<CourseStudentsAddCommand, Result>
{
    public async Task<Result> Handle(CourseStudentsAddCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.ExistsAsync(x => x.Id == request.Id);

        if (!course)
            return Result.Failure("Can't find course");

        var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);

        var emails = await _excel.ProcessEmailListFile(request.File);

        if (emails != null && emails.Any()) 
        {
            foreach (var email in emails) 
            {
                var user = users.SingleOrDefault(x => x.Email == email);

                if (user is null)
                    continue;

                var newUserCourse = new UserCourse(
                    Guid.NewGuid(),
                    user.Id,
                    request.Id,
                    DateTimeOffset.UtcNow.AddYears(1),
                    _current.UserId);

                await _unitOfWork.UserCourseRepository.AddAsync(newUserCourse);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Users added successfully");
    }
}