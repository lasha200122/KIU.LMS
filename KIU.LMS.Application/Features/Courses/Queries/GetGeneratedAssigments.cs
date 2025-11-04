using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetGeneratedAssignmentsQuery(string Name, int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedEntities<GetGeneratedAssignmentsResult>>>;

public sealed record GetGeneratedAssignmentsResult(
    Guid Id, string Name,
    int Count, List<string> Models,
    string Task, string Difficulty,
    GeneratingStatus Status, DateTimeOffset? CompletedAt);

public sealed class GetGeneratedAssignmentsHandler(
    IGeneratedAssignmentRepository repository)
    : IRequestHandler<GetGeneratedAssignmentsQuery, Result<PagedEntities<GetGeneratedAssignmentsResult>>>
{
    public async Task<Result<PagedEntities<GetGeneratedAssignmentsResult>>> Handle(GetGeneratedAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = repository.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(name));
        }
        
        var count = query.Count();

        var result = await query
            .OrderBy(x => x.CreateDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetGeneratedAssignmentsResult(
                x.Id, x.Title, 
                x.Count, x.Models,
                x.TaskContent, x.Difficulty.ToString(),
                x.Status, x.CompletedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedEntities<GetGeneratedAssignmentsResult>(result, count);
    }
}