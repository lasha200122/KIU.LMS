namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModulesQuery(Guid CourseId, string? Name, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetModulesQueryResponse>>>;

public sealed record GetModulesQueryResponse(Guid Id, string Name);

public class GetModulesQueryValidator : AbstractValidator<GetModulesQuery>
{
    public GetModulesQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetModulesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetModulesQuery, Result<PagedEntities<GetModulesQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetModulesQueryResponse>>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.ModuleRepository.GetPaginatedWhereMappedAsync(
            x => x.CourseId == request.CourseId && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)),
            request.PageNumber,
            request.PageSize,
            x => new GetModulesQueryResponse(x.Id, x.Name),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetModulesQueryResponse>>.Success(courses)!;
    }
}