
namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteCourseMeetingCommand(Guid Id) : IRequest<Result>;

public class DeleteCourseMeetingCommandValidator : AbstractValidator<DeleteCourseMeetingCommand> 
{
    public DeleteCourseMeetingCommandValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}

public class DeleteCourseMeetingCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteCourseMeetingCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseMeetingCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _unitOfWork.CourseMeetingRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (meeting is null)
            return Result.Failure("Can't find meeting");

        meeting.Delete(_current.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Meeting deleted successfully");
    }
}