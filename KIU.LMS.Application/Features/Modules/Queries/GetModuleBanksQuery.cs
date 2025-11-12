using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModuleBanksQuery(
    Guid ModuleId,
    int PageNumber,
    int PageSize,
    SubModuleType Type
    ) : IRequest<Result<PagedEntities<GetModuleBanksResponse>>>;

public sealed record GetModuleBanksResponse(
    string ModuleName,
    Guid ModuleId,
    Dictionary<string, int> ModuleTypesAndCount);

public sealed class GetModuleBanksQueryValidator : AbstractValidator<GetModuleBanksQuery>
{
    public GetModuleBanksQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}

public sealed class GetModuleBanksHandler(
    IUnitOfWork unitOfWork,
    IValidator<GetModuleBanksQuery> validator)
    : IRequestHandler<GetModuleBanksQuery, Result<PagedEntities<GetModuleBanksResponse>>>
{
    public async Task<Result<PagedEntities<GetModuleBanksResponse>>> Handle(GetModuleBanksQuery request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        
        if (!result.IsValid)
            return Result<PagedEntities<GetModuleBanksResponse>>.Failure("Validation Failed, you must pass a valid query parameter");
        
        var query = unitOfWork.ModuleBankRepository
            .AsQueryable()
            .Where(x => x.ModuleId == request.ModuleId && x.Type == request.Type)
            .Include(x => x.SubModules);

        var totalCount = await query.CountAsync(cancellationToken);

        var moduleBanks = await query
            .OrderBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var data = moduleBanks.Select(x => new GetModuleBanksResponse(
                x.Name,
                x.ModuleId,
                x.SubModules.Count != 0
                    ? x.SubModules
                        .Where(sm => sm.Difficulty != null)
                        .GroupBy(sm => sm.Difficulty!.Value.ToString())
                        .ToDictionary(g => g.Key, g => g.Count())
                    : new Dictionary<string, int>()))
            .ToList();

        return Result<PagedEntities<GetModuleBanksResponse>>.Success(
            new PagedEntities<GetModuleBanksResponse>(
                data, 
                totalCount));
    }
}
