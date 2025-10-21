namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record RescheduleQuizCommand(Guid Id, DateTimeOffset StartDateTime, DateTimeOffset EndDateTime) : IRequest<Result>;

public sealed class RescheduleQuizCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RescheduleQuizCommand, Result>
{
    public async Task<Result> Handle(RescheduleQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await unitOfWork.QuizRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (quiz is null)
        {
            return Result<Unit>.Failure("Quiz not found.");
        }

        quiz.Reschedule(request.StartDateTime, request.EndDateTime);

        unitOfWork.QuizRepository.Update(quiz);

        await unitOfWork.SaveChangesAsync();

        return Result<Unit>.Success("Quiz updated Successfully");
    }
}
