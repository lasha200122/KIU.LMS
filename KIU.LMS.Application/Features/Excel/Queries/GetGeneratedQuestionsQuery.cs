using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GetGeneratedQuestionsQuery(Guid GeneratedAssigmentId) : IRequest<Result<byte[]>>;

public sealed class GetGeneratedQuestionsHandler(
    IExcelProcessor excelProcessor,
    IGeneratedQuestionRepository generatedQuestionRepository) : IRequestHandler<GetGeneratedQuestionsQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetGeneratedQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await generatedQuestionRepository
            .GetWhereAsync(x => x.GeneratedAssignmentId == request.GeneratedAssigmentId, cancellationToken);

        var stream = new MemoryStream();
        excelProcessor.GetGeneratedAssigmentQuestions(stream, questions);

        return stream.ToArray();
    }
}
