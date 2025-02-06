namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetQuizDetailsQuery(Guid Id) : IRequest<Result<GetQuizDetailsQueryResponse>>;

public sealed record GetQuizDetailsQueryResponse(
    Guid Id,
    string Title,
    bool Allowed,
    int? Attempts,
    int Attempted,
    string StartDateTime,
    string EndDateTime,
    string LastScore,
    int TotalQuestions,
    int? TimePerQuestion,
    string TotalTime,
    List<ExamHistory> History);


public sealed record ExamHistory(
    string SessionId,
    string StartedAt,
    string FinishedAt,
    string Score,
    string TotalQuestions,
    string CorrectAnswers,
    string Duration);


public sealed class GetQuizDetailsQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetQuizDetailsQuery, Result<GetQuizDetailsQueryResponse>>
{
    public async Task<Result<GetQuizDetailsQueryResponse>> Handle(GetQuizDetailsQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.QuizBanks, x => x.ExamResults);

        if (quiz is null)
            return Result<GetQuizDetailsQueryResponse>.Failure("Can't find Id");

        var userAttempts = quiz.ExamResults.Where(x => x.StudentId == _current.UserId).ToList();

        int attempted = userAttempts.Count();

        int totalQuestions = quiz.QuizBanks.Sum(x => x.Amount);

        string totalTime = quiz.TimePerQuestion.HasValue ? $"{(totalQuestions * quiz.TimePerQuestion.Value) % 60 + 1}" : string.Empty;

        bool isAllowed = (!quiz.Attempts.HasValue || quiz.Attempts.Value > attempted) && (!quiz.EndDateTime.HasValue || DateTimeOffset.UtcNow < quiz.EndDateTime.Value);

        var result = new GetQuizDetailsQueryResponse(
            quiz.Id,
            quiz.Title,
            isAllowed,
            quiz.Attempts,
            attempted,
            quiz.StartDateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            quiz.EndDateTime.HasValue ? quiz.EndDateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : string.Empty,
            userAttempts.OrderByDescending(x => x.CreateDate).Select(x => $"{x.Score}").FirstOrDefault() ?? string.Empty,
            totalQuestions,
            quiz.TimePerQuestion,
            totalTime,
            userAttempts.Select(x => new ExamHistory(
                x.SessionId,
                x.StartedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                x.FinishedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                x.Score.ToString(),
                x.TotalQuestions.ToString(),
                x.CorrectAnswers.ToString(),
                x.Duration.ToString())
            ).ToList());

        return Result<GetQuizDetailsQueryResponse>.Success(result);
    }
}