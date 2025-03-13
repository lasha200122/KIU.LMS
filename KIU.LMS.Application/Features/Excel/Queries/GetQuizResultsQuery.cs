using KIU.LMS.Domain.Common.Interfaces.Services;
using KIU.LMS.Domain.Common.Models.Excel;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GetQuizResultsQuery(Guid Id) : IRequest<Result<byte[]>>;


public sealed class GetQuizResultsQueryHandler(IUnitOfWork unitOfWork, IExcelProcessor excelProcessor) : IRequestHandler<GetQuizResultsQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetQuizResultsQuery request, CancellationToken cancellationToken)
    {
        var quizResults = await unitOfWork.ExamResultRepository.GetMappedAsync(
            x => x.QuizId == request.Id,
            x => new QuizResultDto(
                x.User.FirstName,
                x.User.LastName,
                x.User.Email,
                x.StartedAt,
                x.FinishedAt,
                x.Score,
                x.TotalQuestions,
                x.CorrectAnswers,
                x.Duration,
                x.Quiz.MinusScore));

        if (!quizResults.Any())
        {
            return Result<byte[]>.Failure("No quiz results found");
        }

        using (var stream = new MemoryStream())
        {
            excelProcessor.GenerateQuizResults(stream, quizResults);
            return Result<byte[]>.Success(stream.ToArray());
        }
    }
}