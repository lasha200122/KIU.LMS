namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record StartQuizCommand(Guid Id) : IRequest<Result>;

public sealed class StartQuizCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<StartQuizCommand, Result>
{
    public async Task<Result> Handle(StartQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.QuizRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (quiz is null)
        {
            return Result<Unit>.Failure("Quiz not found.");
        }

        quiz.StartQuiz();

        unitOfWork.QuizRepository.Update(quiz);

        await unitOfWork.SaveChangesAsync();

        return Result<Unit>.Success("Quiz Started Successfully");
    }
}