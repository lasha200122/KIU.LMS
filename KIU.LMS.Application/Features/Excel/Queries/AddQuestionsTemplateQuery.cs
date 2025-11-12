namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record AddQuestionsTemplateQuery : IRequest<Result<byte[]>>;

public class AddQuestionsTemplateQueryHandler(IExcelProcessor _excel) : IRequestHandler<AddQuestionsTemplateQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(AddQuestionsTemplateQuery request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        _excel.GenerateQuestionsTemplate(stream);

        return Result<byte[]>.Success(stream.ToArray());
    }
}
