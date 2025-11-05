using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GetExcelGeneratedQuestionsQuery(Guid GeneratedAssigmentId) : IRequest<Result<byte[]>>;

public sealed class GetGeneratedQuestionsHandler(
    IExcelProcessor excelProcessor,
    IGeneratedQuestionRepository generatedQuestionRepository) : IRequestHandler<GetExcelGeneratedQuestionsQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetExcelGeneratedQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await generatedQuestionRepository
            .GetWhereAsync(x => x.GeneratedAssignmentId == request.GeneratedAssigmentId, cancellationToken);

        var stream = new MemoryStream();
        excelProcessor.GetGeneratedAssigmentQuestions(stream, questions);

        return stream.ToArray();
    }
}
