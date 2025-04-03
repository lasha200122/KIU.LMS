using KIU.LMS.Domain.Common.Models.Excel;
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GetQuizResultsQuery(Guid Id) : IRequest<Result<byte[]>>;


public sealed class GetQuizResultsQueryHandler(IUnitOfWork _unitOfWork, IExcelProcessor excelProcessor, IMongoRepository<ExamSession> _sessionRepository,
    IMongoRepository<StudentAnswer> _answerRepository) : IRequestHandler<GetQuizResultsQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetQuizResultsQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (quiz is null)
        {
            return Result<byte[]>.Failure("No quiz results found");
        }

        var results = new List<QuizResultDto>();

        var students = await _unitOfWork.UserCourseRepository.GetWhereIncludedAsync(x => x.CourseId == quiz.CourseId, x => x.User);

        foreach (var student in students)
        {
            var examSessions = await _sessionRepository.FindAsync(x => x.StudentId == student.UserId.ToString() && x.QuizId == request.Id.ToString().ToLower());

            if (!examSessions.Any())
            {
                var defaultResponse = new QuizResultDto(
                    Rank: 0,
                    FirstName: student.User.FirstName,
                    LastName: student.User.LastName,
                    Email: student.User.Email,
                    Institution: student.User.Institution ?? string.Empty,
                    StartedAt: DateTimeOffset.UtcNow,
                    FinishedAt: DateTimeOffset.UtcNow,
                    Score: 0,
                    Percentage: 0,
                    TotalQuestions: 0,
                    CorrectAnswers: 0,
                    WrongAnswers: 0,
                    Duration: TimeSpan.Zero,
                    MinusPoint: quiz.MinusScore,
                    Bonus: 0,
                    FinalScore: 0);

                results.Add(defaultResponse);
                continue;
            }

            foreach (var session in examSessions)
            {
                var answers = await _answerRepository.FindAsync(a => a.SessionId == session.Id);
                var score = CalculateScore(answers.ToList(), session.Questions, quiz.MinusScore);
                var count = CountAnswers(answers.ToList(), session.Questions);
                var totalCount = session.Questions.Count;
                var percentage = totalCount > 0 ? Math.Round((decimal)count.CorrectCount / totalCount * 100, 1) : 0;
                var finishedAt = session.FinishedAt.HasValue ? session.FinishedAt.Value : DateTimeOffset.UtcNow;
                TimeSpan duration = finishedAt - session.StartedAt;
                var minutes = (decimal)duration.TotalMinutes;

                var totalAllowedMinutes = totalCount * (quiz.TimePerQuestion ?? 0) / 60.0m;

                var timeBonus = Math.Round((decimal)(totalAllowedMinutes - minutes) / 15, 2);

                var bonus = Math.Max(0, timeBonus);
                bonus = bonus > count.CorrectCount ? count.CorrectCount : bonus;
                var finalScore = score + bonus;

                var result = new QuizResultDto(
                   Rank: 0,
                   FirstName: student.User.FirstName,
                   LastName: student.User.LastName,
                   Email: student.User.Email,
                   Institution: student.User.Institution ?? string.Empty,
                   StartedAt: session.StartedAt,
                   FinishedAt: finishedAt,
                   Score: score,
                   Percentage: percentage,
                   TotalQuestions: totalCount,
                   CorrectAnswers: count.CorrectCount,
                   WrongAnswers: count.WrongCount,
                   Duration: duration,
                   MinusPoint: quiz.MinusScore,
                   Bonus: bonus,
                   FinalScore: finalScore);


                results.Add(result);
            }
        }

        results = results
            .OrderByDescending(x => x.FinalScore)
            .Select((item, index) => new QuizResultDto(
                Rank: index + 1,
                   FirstName: item.FirstName,
                   LastName: item.LastName,
                   Email: item.Email,
                   Institution: item.Institution,
                   StartedAt: item.StartedAt,
                   FinishedAt: item.FinishedAt,
                   Score: item.Score,
                   Percentage: item.Percentage,
                   TotalQuestions: item.TotalQuestions,
                   CorrectAnswers: item.CorrectAnswers,
                   WrongAnswers: item.WrongAnswers,
                   Duration: item.Duration,
                   MinusPoint: item.MinusPoint,
                   Bonus: item.Bonus,
                   FinalScore: item.FinalScore))
            .ToList();


        using (var stream = new MemoryStream())
        {
            excelProcessor.GenerateQuizResults(stream, results);
            return Result<byte[]>.Success(stream.ToArray());
        }
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