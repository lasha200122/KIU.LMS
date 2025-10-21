namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateCourseTopicCommand(Guid Id, string Name, DateTimeOffset? StartDate, DateTimeOffset? EndDate) : IRequest<Result>;


public class UpdateCourseTopicCommandValidator : AbstractValidator<UpdateCourseTopicCommand>
{
    public UpdateCourseTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();
    }
}

public sealed class UpdateCourseTopicCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<UpdateCourseTopicCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await unitOfWork.TopicRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (topic is null)
            return Result.Failure("Can't find topic");

        topic.Update(
            request.Name,
            request.StartDate,
            request.EndDate,
            current.UserId);

        unitOfWork.TopicRepository.Update(topic);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Topic Updated successfully");
    }
}