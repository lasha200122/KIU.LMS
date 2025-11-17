namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateAssignmentCommand(
    Guid Id,
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
    SolutionType SolutionType,
    bool FullScreen,
    int? RuntimeAttempt,
    bool IsTraining,
    Guid? PromptId,
    string? PromptText,
    string? CodeSolution) : IRequest<Result>;


public class UpdateAssignmentCommandValidator : AbstractValidator<UpdateAssignmentCommand>
{
    public UpdateAssignmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Type)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();

        RuleFor(x => x.Order)
            .NotNull();

        RuleFor(x => x.SolutionType)
            .IsInEnum();
    }
}


public class UpdateAssignmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<UpdateAssignmentCommand, Result>
{
    public async Task<Result> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.AssignmentRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (assignment == null)
            return Result.Failure("Can't find assignment");

        assignment.Update(
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
            request.PromptId,
            request.FullScreen,
            request.RuntimeAttempt,
            request.IsTraining,
            request.PromptText,
            request.CodeSolution,
            current.UserId);

        unitOfWork.AssignmentRepository.Update(assignment);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment Updated Successfully");
    }
}