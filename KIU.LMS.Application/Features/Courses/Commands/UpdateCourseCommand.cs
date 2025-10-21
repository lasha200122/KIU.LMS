namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateCourseCommand(Guid Id, string Name) : IRequest<Result>;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}

public class UpdateCourseCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<UpdateCourseCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim().ToLower();

        if (await unitOfWork.CourseRepository.ExistsAsync(x => x.Name == name, cancellationToken))
            return Result.Failure("Course name should be unique");

        var course = await unitOfWork.CourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (course is null)
            return Result.Failure("Course not found");

        course.Update(name, currentUser.UserId);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Course Updated Successfully");
    }
}