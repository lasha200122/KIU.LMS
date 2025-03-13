namespace KIU.LMS.Domain.Common.Models.Excel;

public sealed record QuizResultDto(
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    decimal Score,
    int TotalQuestions,
    int CorrectAnswers,
    TimeSpan Duration,
    decimal? MinusPoint);
