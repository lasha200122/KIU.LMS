namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteCourseCommand(Guid Id) : IRequest<Result>;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}

public class DeleteCourseCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<DeleteCourseCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await unitOfWork.CourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (course is null)
            return Result.Failure("Course not found");

        course.Delete(currentUser.UserId, DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Course Deleted Successfully");
    }
}