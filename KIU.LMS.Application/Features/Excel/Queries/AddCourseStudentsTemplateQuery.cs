namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record AddCourseStudentsTemplateQuery() : IRequest<Result<byte[]>>;

public class AddCourseStudentsTemplateQueryHandler(IExcelProcessor _excel) : IRequestHandler<AddCourseStudentsTemplateQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(AddCourseStudentsTemplateQuery request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        _excel.GenerateEmailListTemplate(stream);

        return Result<byte[]>.Success(stream.ToArray());
    }
}