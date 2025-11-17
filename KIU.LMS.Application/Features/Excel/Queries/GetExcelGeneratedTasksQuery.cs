using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Excel.Queries;

public sealed record GetExcelGeneratedTasksQuery(
    Guid GeneratedAssignmentId) : IRequest<Result<byte[]>>;

public sealed class GetExcelGeneratedTasksHandler(
    IGeneratedAssignmentRepository repository,
    IExcelProcessor excelProcessor)
    : IRequestHandler<GetExcelGeneratedTasksQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetExcelGeneratedTasksQuery request, CancellationToken cancellationToken)
    {
        var generatedAssignment = await repository
            .AsQueryable()
            .Where(x => x.Id == request.GeneratedAssignmentId)
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (generatedAssignment?.Tasks is null)
            return Result<byte[]>.Failure("Generated tasks not found");
        
        var stream = new MemoryStream();
        
        excelProcessor.GetGeneratedAssignmentTasks(stream, generatedAssignment.Tasks, generatedAssignment.Type, generatedAssignment.Difficulty);
        
        return Result<byte[]>.Success(stream.ToArray());
    }
}