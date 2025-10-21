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

public class CourseDeleteStudentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<CourseDeleteStudentCommand, Result>
{
    public async Task<Result> Handle(CourseDeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserCourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (user is null)
            return Result.Failure("Can't find user");

        user.Delete(current.UserId, DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Student deleted successfully");
    }
}

