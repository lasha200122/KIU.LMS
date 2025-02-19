namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateCourseTopicCommand(Guid Id, string Name, DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<Result>;


public class UpdateCourseTopicCommandValidator : AbstractValidator<UpdateCourseTopicCommand>
{
    public UpdateCourseTopicCommandValidator()
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

public sealed class UpdateCourseTopicCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateCourseTopicCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await _unitOfWork.TopicRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (topic is null)
            return Result.Failure("Can't find topic");

        topic.Update(
            request.Name,
            request.StartDate,
            request.EndDate,
            _current.UserId);

        _unitOfWork.TopicRepository.Update(topic);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Topic Updated successfully");
    }
}