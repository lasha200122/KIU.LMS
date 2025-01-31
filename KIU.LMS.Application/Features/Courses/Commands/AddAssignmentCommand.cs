namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddAssignmentCommand(
    Guid CourseId,
    Guid TopicId,
    AssignmentType Type,
    string Name,
    int Order,
    DateTimeOffset? StartDateTime,
    DateTimeOffset? EndDateTime,
    decimal? Score,
    string? Problem,
    string? Code,
    bool IsPublic,
    bool AiGrader,
    SolutionType SolutionType) : IRequest<Result>;


public class AddAssignmentCommandValidator : AbstractValidator<AddAssignmentCommand> 
{
    public AddAssignmentCommandValidator() 
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.Type)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();

        RuleFor(x => x.Order)
            .NotNull();

        RuleFor(x => x.SolutionType)
            .NotNull();
    }
}


public class AddAssignmentCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<AddAssignmentCommand, Result>
{
    public async Task<Result> Handle(AddAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = new Assignment(
            Guid.NewGuid(),
            request.CourseId,
            request.TopicId,
            request.Type,
            request.Name,
            request.Order,
            request.StartDateTime,
            request.EndDateTime,
            request.Score,
            request.Problem,
            request.Code,
            string.Empty,
            request.IsPublic,
            request.AiGrader,
            request.SolutionType,
            _current.UserId);
    
        await _unitOfWork.AssignmentRepository.AddAsync(assignment);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment Created Successfully");
    }
}