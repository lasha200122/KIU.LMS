namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddCourseTopicCommand(Guid Id, string Name, DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<Result>;


public class AddCourseTopicCommandValidator : AbstractValidator<AddCourseTopicCommand> 
{
    public AddCourseTopicCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .NotNull();
    }
}

public sealed class AddCourseTopicCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<AddCourseTopicCommand, Result>
{
    public async Task<Result> Handle(AddCourseTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = new Topic(
            Guid.NewGuid(),
            request.Id,
            request.Name,
            request.StartDate,
            request.EndDate,
            _current.UserId);

        await _unitOfWork.TopicRepository.AddAsync(topic);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Topic Create successfully");
    }
}