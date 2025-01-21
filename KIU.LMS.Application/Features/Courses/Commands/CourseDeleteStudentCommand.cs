namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record CourseDeleteStudentCommand(Guid Id) : IRequest<Result>;

public class CourseDeleteStudentCommandValidator : AbstractValidator<CourseDeleteStudentCommand>
{
    public CourseDeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}

public class CourseDeleteStudentCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CourseDeleteStudentCommand, Result>
{
    public async Task<Result> Handle(CourseDeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserCourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (user is null)
            return Result.Failure("Can't find user");

        user.Delete(_current.UserId, DateTimeOffset.UtcNow);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Student deleted successfully");
    }
}

