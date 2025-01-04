namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record RegisterStudentsTemplateQuery() : IRequest<Result<byte[]>>;

public class RegisterStudentsTemplateQueryHandler(IExcelProcessor _excel) : IRequestHandler<RegisterStudentsTemplateQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(RegisterStudentsTemplateQuery request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        _excel.GenerateStudentRegistrationTemplate(stream);

        return Result<byte[]>.Success(stream.ToArray());
    }
}