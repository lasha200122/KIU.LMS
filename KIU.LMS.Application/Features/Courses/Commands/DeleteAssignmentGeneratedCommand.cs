namespace KIU.LMS.Application.Features.Courses.Commands;
public sealed record DeleteAssignmentGeneratedCommand(Guid Id) : IRequest<Result>;


public sealed class DeleteGeneratedAssignmentHandler
    (IUnitOfWork unitOfWork, ICurrentUserService current)
    : IRequestHandler<DeleteAssignmentGeneratedCommand, Result>
{
    
    public async Task<Result> Handle(DeleteAssignmentGeneratedCommand request, CancellationToken cancellationToken)
    {
        var generatedAssignment = await unitOfWork.GeneratedAssignmentRepository
            .SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (generatedAssignment is null)
            return Result.Failure("Can't find generated-assignment");   
        
        generatedAssignment.Delete(current.UserId, DateTimeOffset.UtcNow);

        unitOfWork.GeneratedAssignmentRepository.Update(generatedAssignment);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Generated-Assignment deleted successfully");
    }
}
