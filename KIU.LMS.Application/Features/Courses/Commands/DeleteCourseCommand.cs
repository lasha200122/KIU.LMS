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
    IUnitOfWork _unitOfWork,
    ICurrentUserService _currentUser) : IRequestHandler<DeleteCourseCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (course is null)
            return Result.Failure("Course not found");

        course.Delete(_currentUser.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Course Deleted Successfully");
    }
}