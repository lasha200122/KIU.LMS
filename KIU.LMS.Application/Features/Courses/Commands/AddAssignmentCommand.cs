namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddAssignmentCommand(
    Guid CourseId,
    Guid TopicId,
    AssignmentType Type,
    string Name,
    DateTimeOffset? StartDateTime,
    DateTimeOffset? EndDateTime,
    decimal? Score,
    string? Problem,
    string? Code,
    bool IsPublic,
    bool AiGrader,
    SolutionType SolutionType,
    bool FullScreen,
    int? RuntimeAttempt,
    Guid? PromptId,
    bool IsTraining,
    string? PromptText,
    string? CodeSolution) : IRequest<Result>;

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

        RuleFor(x => x.SolutionType)
            .NotNull();
    }
}

public class AddAssignmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService _current) : IRequestHandler<AddAssignmentCommand, Result>
{
    public async Task<Result> Handle(AddAssignmentCommand request, CancellationToken cancellationToken)
    {
        var splitted = request.Name.Split(".");
        string name = string.Empty;
        int order = 0;

        if (splitted.Count() == 1)
            name = splitted.First();
        else 
        {
            name = splitted.Last();
            int.TryParse(splitted.First(), out order);
        }

        var assignment = new Assignment(
            Guid.NewGuid(),
            request.CourseId,
            request.TopicId,
            request.Type,
            name,
            order,
            request.StartDateTime,
            request.EndDateTime,
            request.Score,
            request.Problem,
            request.Code,
            string.Empty,
            request.IsPublic,
            request.AiGrader,
            request.SolutionType,
            request.PromptId,
            request.FullScreen,
            request.RuntimeAttempt,
            request.IsTraining,
            request.PromptText,
            request.CodeSolution,
            _current.UserId);
    
        await unitOfWork.AssignmentRepository.AddAsync(assignment);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment Created Successfully");
    }
}