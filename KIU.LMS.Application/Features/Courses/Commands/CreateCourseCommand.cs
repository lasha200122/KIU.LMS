namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record CreateCourseCommand(string Name) : IRequest<Result>;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}

public class CreateCourseCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<CreateCourseCommand, Result>
{
    public async Task<Result> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var courseExist = await unitOfWork.CourseRepository.ExistsAsync(x => x.Name == name, cancellationToken);

        if (courseExist)
            return Result.Failure("Course is already created");

        var newCourse = new Course(
            Guid.NewGuid(),
            name,
            currentUser.UserId);

        await unitOfWork.CourseRepository.AddAsync(newCourse);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Course Created Successfully");
    }
}