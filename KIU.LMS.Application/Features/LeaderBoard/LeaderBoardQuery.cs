using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.LeaderBoard;

public sealed record LeaderBoardQuery(Guid CourseId, Guid QuizId, int Page, string? Text) : IRequest<Result<FinalResponse>>;

public sealed record FinalResponse(List<LeaderBoardResponse> Students, int Count);
public sealed record LeaderBoardResponse(
    int Rank,
    string FirstName,
    string LastName,
    string School,
    decimal Percentage,
    int CorrectAnswers,
    int TotalAnswers,
    decimal Score,
    string Duration,
    decimal Bonus,
    decimal TotalScore);

public sealed class LeaderBoardQueryHandler(
    IUnitOfWork _unitOfWork,
    IMongoRepository<ExamSession> _sessionRepository,
    IMongoRepository<StudentAnswer> _answerRepository) : IRequestHandler<LeaderBoardQuery, Result<FinalResponse>>
{
    public async Task<Result<FinalResponse>> Handle(LeaderBoardQuery request, CancellationToken cancellationToken)
    {
        var students = await _unitOfWork.UserCourseRepository.GetWhereIncludedAsync(
            x => x.CourseId == request.CourseId &&
            (string.IsNullOrEmpty(request.Text) || x.User.FirstName.Contains(request.Text) || x.User.LastName.Contains(request.Text) || x.User.Email.Contains(request.Text) || x.User.Institution.Contains(request.Text))
            , x => x.User);

        var response = new List<LeaderBoardResponse>();

        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == request.QuizId);

        var finalResponse_ = new FinalResponse(response, 0);

        if (quiz is null)
            return Result<FinalResponse>.Success(finalResponse_);

        foreach (var student in students)
        {
            var examSessions = await _sessionRepository.FindAsync(x => x.StudentId == student.UserId.ToString() && x.QuizId == request.QuizId.ToString());

            if (!examSessions.Any())
            {
                var defaultResponse = new LeaderBoardResponse(
                    Rank: 0,
                    FirstName: student.User.FirstName,
                    LastName: student.User.LastName,
                    School: student.User.Institution ?? string.Empty,
                    Percentage: 0,
                    CorrectAnswers: 0,
                    TotalAnswers: 0,
                    Score: 0,
                    Bonus: 0,
                    Duration: string.Empty,
                    TotalScore:0);

                response.Add(defaultResponse);
                continue;
            }

            foreach (var session in examSessions)
            {
                var answers = await _answerRepository.FindAsync(a => a.SessionId == session.Id);

                var score = CalculateScore(answers.ToList(), session.Questions, quiz.MinusScore);
                var count = CountAnswers(answers.ToList(), session.Questions);
                var totalCount = session.Questions.Count;
                var percentage = totalCount > 0 ? Math.Round((decimal)count.CorrectCount / totalCount * 100, 1) : 0;
                var finishedAt = session.FinishedAt.HasValue? session.FinishedAt.Value : DateTimeOffset.UtcNow;
                TimeSpan duration = finishedAt - session.StartedAt;
                var minutes = duration.TotalMinutes;
                var givenMinutes = (answers.Select(x => x.QuestionId).Distinct().Count() * quiz.TimePerQuestion?? 0) / 60;
                var bonus = Math.Round((decimal) (givenMinutes - minutes )/ 15, 2);
                bonus = bonus > count.CorrectCount ? count.CorrectCount : bonus;
                var finalScore = score + bonus;

                var result = new LeaderBoardResponse(
                    Rank: 0,
                    FirstName: student.User.FirstName,
                    LastName: student.User.LastName,
                    School: student.User.Institution ?? string.Empty,
                    Percentage: percentage,
                    CorrectAnswers: count.CorrectCount,
                    TotalAnswers: totalCount,
                    Score: score,
                    Bonus: bonus,
                    TotalScore: finalScore,
                    Duration: duration.ToString());

                response.Add(result);
            }
        }

        var final_count = response.Count();

        var skip = (request.Page - 1) * 10;

        response = response
            .OrderByDescending(x => x.TotalScore)
            .Select((item, index) => new LeaderBoardResponse(
                Rank: index + 1,
                FirstName: item.FirstName,
                LastName: item.LastName,
                School: item.School,
                Percentage: item.Percentage,
                CorrectAnswers: item.CorrectAnswers,
                TotalAnswers: item.TotalAnswers,
                Score: item.Score, 
                Bonus: item.Bonus,
                TotalScore: item.TotalScore,
                Duration: item.Duration))
            .Skip(skip)
            .Take(10)
            .ToList();

        var finalResponse = new FinalResponse(response, final_count);

        return Result<FinalResponse>.Success(finalResponse);
    }

    private decimal CalculateScore(List<StudentAnswer> answers, List<Domain.Entities.NoSQL.ExamQuestion> questions, decimal? penaltyPerWrongAnswer = null)
    {
        var (correctCount, wrongCount) = CountAnswers(answers, questions);

        if (questions.Count <= 0)
            return 0;

        decimal baseScore = (decimal)correctCount;

        if (penaltyPerWrongAnswer.HasValue && penaltyPerWrongAnswer.Value > 0)
        {
            decimal penalty = wrongCount * penaltyPerWrongAnswer.Value;
            return baseScore - penalty;
        }

        return baseScore;
    }

    private (int CorrectCount, int WrongCount) CountAnswers(List<StudentAnswer> answers, List<Domain.Entities.NoSQL.ExamQuestion> questions)
    {
        int correctCount = 0;
        int wrongCount = 0;

        foreach (var answer in answers)
        {
            var question = questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
            if (question != null)
            {
                var correctOptions = question.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.Id)
                    .ToList();

                if (answer.SelectedOptions.Count == correctOptions.Count &&
                    answer.SelectedOptions.All(correctOptions.Contains))
                {
                    correctCount++;
                }
                else
                {
                    wrongCount++;
                }
            }
        }

        return (correctCount, wrongCount);
    }
}