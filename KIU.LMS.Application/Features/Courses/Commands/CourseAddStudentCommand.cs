namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record CourseAddStudentCommand(Guid CourseId, Guid UserId, DateTimeOffset CanAccessTill) : IRequest<Result>;

public class CourseAddStudentCommandValidator : AbstractValidator<CourseAddStudentCommand> 
{
    public CourseAddStudentCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.UserId)
            .NotNull();
    }
}

public class CourseAddStudentCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CourseAddStudentCommand, Result>
{
    public async Task<Result> Handle(CourseAddStudentCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.ExistsAsync(x => x.Id == request.UserId);

        if (!user)
            return Result.Failure("Can't find user");

        var course = await _unitOfWork.CourseRepository.ExistsAsync(x => x.Id == request.CourseId);

        if (!course)
            return Result.Failure("Can't find course");

        var newUserCourse = new UserCourse(
            Guid.NewGuid(),
            request.UserId,
            request.CourseId,
            request.CanAccessTill,
            _current.UserId);

        await _unitOfWork.UserCourseRepository.AddAsync(newUserCourse);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("User added successfully");
    }
}