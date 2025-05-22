namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record DeleteQuizCommand(Guid Id)  : IRequest<Result>;

public sealed class DeleteQuizCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteQuizCommand, Result>
{
    public async Task<Result> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (quiz is null)
            return Result<Unit>.Failure("Quiz not found");

        quiz.Delete(_current.UserId, DateTimeOffset.UtcNow);

        _unitOfWork.QuizRepository.Update(quiz);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Quiz deleted successfully");
    }
}
