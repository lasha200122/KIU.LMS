namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateCourseMeetingCommand(
    Guid Id,
    string Name,
    string? Url,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime) : IRequest<Result>;

public class UpdateCourseMeetingCommandValidator : AbstractValidator<UpdateCourseMeetingCommand>
{
    public UpdateCourseMeetingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.StartDateTime)
            .NotNull();

        RuleFor(x => x.EndDateTime)
            .NotNull();
    }
}

public class UpdateCourseMeetingCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateCourseMeetingCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseMeetingCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _unitOfWork.CourseMeetingRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (meeting is null)
            return Result.Failure("Can't find meeting");

        meeting.Update(
            request.Name,
            request.Url,
            request.StartDateTime,
            request.EndDateTime,
            _current.UserId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Meeting updated successfully");
    }
}


