namespace KIU.LMS.Application.Features.Courses.Commands;
public sealed record DeleteAssignmentQuestionCommand(Guid Id) : IRequest<Result>;


public sealed class DeleteAssignmentQuestionHandler
    (IUnitOfWork unitOfWork, ICurrentUserService current)
    : IRequestHandler<DeleteAssignmentQuestionCommand, Result>
{
    
    public async Task<Result> Handle(DeleteAssignmentQuestionCommand request, CancellationToken cancellationToken)
    {
        var generatedQuestion = await unitOfWork.GeneratedQuestionRepository
            .SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (generatedQuestion is null)
            return Result.Failure("Can't find generated-question");   
        
        generatedQuestion.Delete(current.UserId, DateTimeOffset.UtcNow);

        unitOfWork.GeneratedQuestionRepository.Update(generatedQuestion);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Generated-question deleted successfully");
    }
}