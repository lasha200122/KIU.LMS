using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetGeneratedTasksQuery(
    Guid GeneratedAssignmentId,
    GeneratedAssignmentType Type, int Page = 1, int PageSize = 10) : IRequest<PagedEntities<GetGeneratedTasksResponse>>;

public sealed record GetGeneratedTasksResponse(
        string TaskDescription,
        string CodeSolution,
        string CodeGenerationPrompt,
        string CodeGradingPrompt, string Difficulty);


public sealed class GetGeneratedTasksHandler(IGeneratedTaskRepository repository)
    : IRequestHandler<GetGeneratedTasksQuery, PagedEntities<GetGeneratedTasksResponse>>
{
    public async Task<PagedEntities<GetGeneratedTasksResponse>> Handle(GetGeneratedTasksQuery request, CancellationToken cancellationToken)
    {
        var query = repository
            .AsQueryable()
            .Where(x => 
                x.GeneratedAssignmentId == request.GeneratedAssignmentId
                && x.Assignment.Type == request.Type);
        
        var count = await query.CountAsync(cancellationToken);
        
        var result = await query
            .OrderBy(x => x.CreateDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetGeneratedTasksResponse(
                x.TaskDescription, x.CodeSolution, x.CodeGenerationPrompt, x.CodeGradingPrompt, x.Assignment.Difficulty.ToString()))
            .ToArrayAsync(cancellationToken);
        
        return new PagedEntities<GetGeneratedTasksResponse>(result, count);
    }
}