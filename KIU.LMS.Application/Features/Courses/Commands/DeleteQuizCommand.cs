namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteQuizCommand(Guid Id)  : IRequest<Result>;

public sealed class DeleteQuizCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<DeleteQuizCommand, Result>
{
    public async Task<Result> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.QuizRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (quiz is null)
            return Result<Unit>.Failure("Quiz not found");

        quiz.Delete(current.UserId, DateTimeOffset.UtcNow);

        unitOfWork.QuizRepository.Update(quiz);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Quiz deleted successfully");
    }
}
