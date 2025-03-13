namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record GradeSolutionCommand(Guid Id) : IRequest<Result>;

public sealed class GradeSolutionCommandHandler(IUnitOfWork _unitOfWork, IGradingService _gradingService, ICurrentUserService _current) : IRequestHandler<GradeSolutionCommand, Result>
{
    public async Task<Result> Handle(GradeSolutionCommand request, CancellationToken cancellationToken)
    {
        var solution = await _unitOfWork.SolutionRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id, x => x.Assignment, x => x.Assignment.Prompt);

        if (solution is null)
            return Result.Failure("Can't find solution");

        await _gradingService.GradeSubmissionAsync(solution);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Solution Graded successfully");
    }
}