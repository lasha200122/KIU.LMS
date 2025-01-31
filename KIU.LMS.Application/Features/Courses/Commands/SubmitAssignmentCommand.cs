namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record SubmitAssignmentCommand(
    Guid Id,
    string Value) : IRequest<Result>;


public class SubmitAssignmentCommandValidator : AbstractValidator<SubmitAssignmentCommand> 
{
    public SubmitAssignmentCommandValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Value)
            .NotNull();
    }
}

public class SubmitAssignmentCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<SubmitAssignmentCommand, Result>
{
    public async Task<Result> Handle(SubmitAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.AssignmentRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (assignment is null)
            return Result.Failure("Can't find assignment");

        if (assignment.EndDateTime.HasValue && assignment.EndDateTime.Value < DateTimeOffset.UtcNow)
            return Result.Failure("Assignment is closed");

        var solution = new Solution(
            Guid.NewGuid(),
            assignment.Id,
            _current.UserId,
            request.Value,
            string.Empty,
            string.Empty,
            GradingStatus.None,
            _current.UserId);

        await _unitOfWork.SolutionRepository.AddAsync(solution);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment Submeted successfully");
    }
}
