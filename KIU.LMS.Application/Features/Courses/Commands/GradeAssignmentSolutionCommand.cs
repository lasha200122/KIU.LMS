namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record GradeAssignmentSolutionCommand(Guid UserId, Guid AssignmentId, string Feedback, string Grade) : IRequest<Result>;

public sealed class GradeAssignmentSolutionCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GradeAssignmentSolutionCommand, Result>
{
    public async Task<Result> Handle(GradeAssignmentSolutionCommand request, CancellationToken cancellationToken)
    {
        var solution = await _unitOfWork.SolutionRepository.SingleOrDefaultWithTrackingAsync(x => x.UserId == request.UserId && x.AssignmentId == request.AssignmentId);
        if (solution is null)
            return Result.Failure("Solution not found");

        if (solution.GradingStatus == GradingStatus.Completed)
            return Result.Failure("Solution already graded");

        solution.Graded(request.Grade, request.Feedback);
        _unitOfWork.SolutionRepository.Update(solution);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success("Solution graded successfully");
    }
}
