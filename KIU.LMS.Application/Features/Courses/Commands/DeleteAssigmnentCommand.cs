
namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteAssigmnentCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteAssigmnentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<DeleteAssigmnentCommand, Result>
{
    public async Task<Result> Handle(DeleteAssigmnentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await unitOfWork.AssignmentRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (assignment is null)
            return Result.Failure("Cant' find assignment");

        assignment.Delete(current.UserId, DateTimeOffset.UtcNow);

        unitOfWork.AssignmentRepository.Update(assignment);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Assignment deleted successfully");
    }
}
