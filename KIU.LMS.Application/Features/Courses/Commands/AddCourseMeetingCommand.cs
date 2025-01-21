namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddCourseMeetingCommand(
    Guid CourseId,
    string Name,
    string Url,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime) : IRequest<Result>;


public class AddCourseMeetingCommandValidator : AbstractValidator<AddCourseMeetingCommand> 
{
    public AddCourseMeetingCommandValidator() 
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Url)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.StartDateTime)
            .NotNull();

        RuleFor(x => x.EndDateTime)
            .NotNull();
    }
}


public class AddCourseMeetingCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<AddCourseMeetingCommand, Result>
{
    public async Task<Result> Handle(AddCourseMeetingCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.SingleOrDefaultAsync(x => x.Id == request.CourseId);

        if (course == null)
            return Result.Failure("Can't find course");

        var meeting = new CourseMeeting(
            Guid.NewGuid(),
            request.CourseId,
            request.Name,
            request.Url,
            request.StartDateTime,
            request.EndDateTime,
            _current.UserId);


        var emailTemplate = await _unitOfWork.EmailTemplateRepository.SingleOrDefaultAsync(x => x.Type == EmailType.MeetingCreated);

        if (emailTemplate != null)
        {
            var emailsToSend = await _unitOfWork.UserCourseRepository.GetMappedAsync(x => x.CourseId == request.CourseId, x => new EmailQueue(
                Guid.NewGuid(),
                emailTemplate.Id,
                x.User.Email,
                EmailTemplateUtils.MeetingCreatedVariableBuilder(x.User, meeting, course.Name),
                _current.UserId));

            if (emailsToSend != null && emailsToSend.Count() > 0)
            {
                await _unitOfWork.EmailQueueRepository.AddRangeAsync(emailsToSend);
            }
        }

        await _unitOfWork.CourseMeetingRepository.AddAsync(meeting);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Meeting Created successfully");
    }
}