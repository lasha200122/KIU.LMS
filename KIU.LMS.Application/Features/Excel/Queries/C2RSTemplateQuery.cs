namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record C2RSTemplateQuery() : IRequest<Result<byte[]>>;


public sealed class C2RSTemplateQueryHandler(IExcelProcessor excelProcessor) : IRequestHandler<C2RSTemplateQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(C2RSTemplateQuery request, CancellationToken cancellationToken)
    {
        using (var stream = new MemoryStream())
        {
            excelProcessor.GenerateC2RSTemplate(stream);
            return Result<byte[]>.Success(stream.ToArray());
        }
    }
}
