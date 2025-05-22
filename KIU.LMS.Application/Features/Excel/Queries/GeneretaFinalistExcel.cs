using KIU.LMS.Application.Features.LeaderBoard;
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GeneretaFinalistExcel(Guid QuizId, Guid CourseId) : IRequest<Result<byte[]>>;


public sealed class GeneretaFinalistExcelHandler(IUnitOfWork _unitOfWork, IMongoRepository<ExamSession> _sessionRepository,
    IMongoRepository<StudentAnswer> _answerRepository, IExcelProcessor _excel) : IRequestHandler<GeneretaFinalistExcel, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GeneretaFinalistExcel request, CancellationToken cancellationToken)
    {
        var students = await _unitOfWork.UserCourseRepository.GetWhereIncludedAsync(
            x => x.CourseId == request.CourseId
            , x => x.User);

        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == request.QuizId);

        var firstQuz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == new Guid("4A9358AF-F19A-4598-9B06-A6D72BFF9A50"));
        var Firststudents = await _unitOfWork.UserCourseRepository.GetWhereIncludedAsync(
            x => x.CourseId == new Guid("83FF3527-5F38-4414-84B1-5C4417D7020A") 
            , x => x.User);

        var scores1 = await GetStudentScores(students, quiz);
        var scores2 = await GetStudentScores(Firststudents, firstQuz);

        var combinedScores = new List<LeaderBoardResponse>();

        combinedScores.AddRange(scores1);

        foreach (var score2 in scores2)
        {
            var existingScore = combinedScores.FirstOrDefault(s =>
                s.FirstName == score2.FirstName &&
                s.LastName == score2.LastName);

            if (existingScore != null)
            {
                int index = combinedScores.IndexOf(existingScore);

                var updatedScore = existingScore with
                {
                    Score = existingScore.Score + score2.Score,
                    TotalScore = existingScore.TotalScore + score2.TotalScore
                };

                combinedScores[index] = updatedScore;
            }
            else
            {
                combinedScores.Add(score2);
            }
        }

        var schoolRanking = combinedScores
            .Where(x => x.Score >= (decimal)13.5 || x.TotalScore >= (decimal)17.39)
            .OrderByDescending(x => x.TotalScore)
            .Select((item, index) => new SchoolRankingItemFinal(
                index + 1,
                $"{item.FirstName} {item.LastName}",
                item.TotalScore.ToString("F2"),
                item.Email))
            .ToList();


        using (var stream = new MemoryStream())
        {
            _excel.GenerateFinalists(stream, schoolRanking);
            return Result<byte[]>.Success(stream.ToArray());
        }
    }


    private async Task<List<LeaderBoardResponse>> GetStudentScores(ICollection<UserCourse> students, Quiz quiz)
    {
        var response = new List<LeaderBoardResponse>();
        foreach (var student in students)
        {
            var examSessions = await _sessionRepository.FindAsync(x => x.StudentId == student.UserId.ToString() && x.QuizId == quiz.Id.ToString());

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
                    TotalScore: 0,
                    Email: student.User.Email);

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
                var finishedAt = session.FinishedAt.HasValue ? session.FinishedAt.Value : DateTimeOffset.UtcNow;
                TimeSpan duration = finishedAt - session.StartedAt;
                var minutes = duration.TotalMinutes;
                var givenMinutes = (answers.Select(x => x.QuestionId).Distinct().Count() * quiz.TimePerQuestion ?? 0) / 60;
                var timeBonus = Math.Round((decimal)(givenMinutes - minutes) / 15, 2);
                var bonus = Math.Max(0, timeBonus);
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
                    Duration: duration.ToString(),
                    Email: student.User.Email);

                response.Add(result);
            }
        }

        return response;
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
