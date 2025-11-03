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
        var course = await unitOfWork.CourseRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);
        
        if (course is null)
            return Result.Failure("Course not found");
        
        var name = request.Name.Trim();

        if (name.Equals(course.Name, StringComparison.CurrentCultureIgnoreCase))
        {
            if(name == course.Name) 
                return Result.Failure($"Course is already named: {name}");
            
            course.Update(name,  currentUser.UserId);
            await unitOfWork.SaveChangesAsync();

            return Result.Success("Course Updated Successfully");
        }
        
        var courseExists = await unitOfWork.CourseRepository.ExistsAsync(x => 
            x.Name.ToLower() == name.ToLower() && 
            name.ToLower() != course.Name.ToLower(),
            cancellationToken); 
        
        if(courseExists) 
            return Result.Failure("Course already exists");
            
        course.Update(name, currentUser.UserId);
        await unitOfWork.SaveChangesAsync();
            
        return Result.Success("Course Updated Successfully");
    }
}