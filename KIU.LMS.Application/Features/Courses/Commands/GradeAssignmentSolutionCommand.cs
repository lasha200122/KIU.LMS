namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record GradeAssignmentSolutionCommand(Guid UserId, Guid AssignmentId, string Feedback, string Grade)
    : IRequest<Result>;

public sealed class GradeAssignmentSolutionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GradeAssignmentSolutionCommand, Result>
{
    public async Task<Result> Handle(GradeAssignmentSolutionCommand request, CancellationToken cancellationToken)
    {
        var solution = await unitOfWork.SolutionRepository.FirstOrDefaultWithTrackingAsync(x =>
            x.UserId == request.UserId &&
            x.AssignmentId == request.AssignmentId, cancellationToken);
        
        if (solution is null)
            return Result.Failure("Solution not found");

        if (solution.GradingStatus == GradingStatus.Completed)
            return Result.Failure("Solution already graded");

        solution.Graded(request.Grade, request.Feedback);
        unitOfWork.SolutionRepository.Update(solution);
        await unitOfWork.SaveChangesAsync();
        return Result.Success("Solution graded successfully");
    }
}