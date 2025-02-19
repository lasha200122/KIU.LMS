
namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteAssigmnentCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteAssigmnentCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteAssigmnentCommand, Result>
{
    public async Task<Result> Handle(DeleteAssigmnentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.AssignmentRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (assignment is null)
            return Result.Failure("Cant' find assignment");

        assignment.Delete(_current.UserId, DateTimeOffset.UtcNow);

        _unitOfWork.AssignmentRepository.Update(assignment);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment deleted successfully");
    }
}
