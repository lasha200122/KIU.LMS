namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddCourseTopicCommand(Guid Id, string Name, DateTimeOffset? StartDate, DateTimeOffset? EndDate) : IRequest<Result>;


public class AddCourseTopicCommandValidator : AbstractValidator<AddCourseTopicCommand> 
{
    public AddCourseTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();
    }
}

public sealed class AddCourseTopicCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<AddCourseTopicCommand, Result>
{
    public async Task<Result> Handle(AddCourseTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = new Topic(
            Guid.NewGuid(),
            request.Id,
            request.Name,
            request.StartDate,
            request.EndDate,
            current.UserId);

        await unitOfWork.TopicRepository.AddAsync(topic);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Topic Create successfully");
    }
}