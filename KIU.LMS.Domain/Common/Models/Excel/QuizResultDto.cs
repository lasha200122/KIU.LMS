namespace KIU.LMS.Domain.Common.Models.Excel;

public sealed record QuizResultDto(
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    decimal Score,
    decimal Percentage,
    int TotalQuestions,
    int CorrectAnswers,
    int WrongAnswers,
    TimeSpan Duration,
    decimal? MinusPoint,
    string? Institution,
    decimal Bonus,
    decimal FinalScore,
    int Rank);
