namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteCourseTopicCommand(Guid Id) : IRequest<Result>;

public class DeleteCourseTopicCommandValidator : AbstractValidator<DeleteCourseTopicCommand>
{
    public DeleteCourseTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}

public class DeleteCourseTopicCommandHandler(
    IUnitOfWork _unitOfWork,
    ICurrentUserService _currentUser) : IRequestHandler<DeleteCourseTopicCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseTopicCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.TopicRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (course is null)
            return Result.Failure("Topic not found");

        course.Delete(_currentUser.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Topic Deleted Successfully");
    }
}