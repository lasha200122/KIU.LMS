namespace KIU.LMS.Application.Features.Modules.Queries;


public sealed record GetSubModulesQuery(
    Guid ModuleId,
    SubModuleType Type,
    string? Name,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedEntities<GetSubModulesQueryResponse>>>;


public sealed record GetSubModulesQueryResponse(Guid Id, string Name);

public class GetSubModulesQueryValidator : AbstractValidator<GetSubModulesQuery>
{
    public GetSubModulesQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetSubModulesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetSubModulesQuery, Result<PagedEntities<GetSubModulesQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetSubModulesQueryResponse>>> Handle(GetSubModulesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.SubModuleRepository.GetPaginatedWhereMappedAsync(
            x => x.ModuleId == request.ModuleId && x.SubModuleType == request.Type && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)),
            request.PageNumber,
            request.PageSize,
            x => new GetSubModulesQueryResponse(x.Id, x.Name),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetSubModulesQueryResponse>>.Success(courses)!;
    }
}