namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModulesBankQuery(
    Guid ModuleId,
    SubModuleType Type,
    string? Name,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedEntities<GetModulesBankQueryResponse>>>;

public sealed record GetModulesBankQueryResponse(Guid Id, string Name);

public class GetSubModulesQueryValidator : AbstractValidator<GetModulesBankQuery>
{
    public GetSubModulesQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetSubModulesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetModulesBankQuery, Result<PagedEntities<GetModulesBankQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetModulesBankQueryResponse>>> Handle(GetModulesBankQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.ModuleBankRepository.GetPaginatedWhereMappedAsync(
            x => x.ModuleId == request.ModuleId && x.Type == request.Type && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)),
            request.PageNumber,
            request.PageSize,
            x => new GetModulesBankQueryResponse(x.Id, x.Name),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetModulesBankQueryResponse>>.Success(courses)!;
    }
}

// New Code 

public sealed record GetSubModulesQuery(
    Guid ModuleBankId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedEntities<GetSubModulesQueryResponse>>>;

public sealed record GetSubModulesQueryResponse(
    Guid Id,
    string? TaskDescription,
    string? CodeSolution,
    string? CodeGenerationPrompt,
    string? CodeGraidingPrompt,
    string? Solution,
    DifficultyType? Difficulty);

public class GetSubModulessQueryValidator : AbstractValidator<GetSubModulesQuery>
{
    public GetSubModulessQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetSubModulesQuerysHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetSubModulesQuery, Result<PagedEntities<GetSubModulesQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetSubModulesQueryResponse>>> Handle(GetSubModulesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.SubModuleRepository.GetPaginatedWhereMappedAsync(
            x => x.ModuleBankId == request.ModuleBankId,
            request.PageNumber,
            request.PageSize,
            x => new GetSubModulesQueryResponse(
                x.Id,
                x.TaskDescription,
                x.CodeSolution,
                x.CodeGenerationPrompt,
                x.CodeGraidingPrompt,
                x.Solution,
                x.Difficulty),
            x => x.CreateDate,
            cancellationToken);

        return Result<PagedEntities<GetSubModulesQueryResponse>>.Success(courses)!;
    }
}

public record SubModuleDto
{
    public Guid Id { get; init; }
    public Guid ModuleBankId { get; init; }
    public string ModuleBankName { get; init; } = string.Empty;
    public string ModuleName { get; init; } = string.Empty;
    public SubModuleType ModuleBankType { get; init; }
    public string? TaskDescription { get; init; }
    public string? CodeSolution { get; init; }
    public string? CodeGenerationPrompt { get; init; }
    public string? CodeGradingPrompt { get; init; }
    public string? Solution { get; init; }
    public DifficultyType? Difficulty { get; init; }
    public string DifficultyDisplay => Difficulty?.ToString() ?? "None";
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public record SubModuleListDto
{
    public Guid Id { get; init; }
    public string? TaskDescription { get; init; }
    public DifficultyType? Difficulty { get; init; }
    public string DifficultyDisplay => Difficulty?.ToString() ?? "None";
    public string ModuleBankName { get; init; } = string.Empty;
    public SubModuleType ModuleBankType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record GetSubModuleByIdQuery(Guid Id) : IRequest<Result<SubModuleDto>>;

public class GetSubModuleByIdQueryValidator : AbstractValidator<GetSubModuleByIdQuery>
{
    public GetSubModuleByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("SubModule Id is required");
    }
}

public sealed class GetSubModuleByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetSubModuleByIdQuery, Result<SubModuleDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<SubModuleDto>> Handle(GetSubModuleByIdQuery request, CancellationToken cancellationToken)
    {
        var subModule = await _unitOfWork.SubModuleRepository
            .SingleOrDefaultAsync(
                predicate: x => x.Id == request.Id,
                x => x.ModuleBank,
                x => x.ModuleBank.Module);

        if (subModule == null)
        {
            return Result<SubModuleDto>.Failure("SubModule not found");
        }

        var dto = new SubModuleDto
        {
            Id = subModule.Id,
            ModuleBankId = subModule.ModuleBankId,
            ModuleBankName = subModule.ModuleBank.Name,
            ModuleName = subModule.ModuleBank.Module.Name,
            ModuleBankType = subModule.ModuleBank.Type,
            TaskDescription = subModule.TaskDescription,
            CodeSolution = subModule.CodeSolution,
            CodeGenerationPrompt = subModule.CodeGenerationPrompt,
            CodeGradingPrompt = subModule.CodeGraidingPrompt,
            Solution = subModule.Solution,
            Difficulty = subModule.Difficulty,
            CreatedAt = subModule.CreateDate,
            UpdatedAt = subModule.LastUpdateDate
        };

        return Result<SubModuleDto>.Success(dto);
    }
}

public sealed record GetSubModulesByModuleQuery(
    Guid ModuleId,
    SubModuleType? Type = null
) : IRequest<Result<List<SubModuleListDto>>>;

public class GetSubModulesByModuleQueryValidator : AbstractValidator<GetSubModulesByModuleQuery>
{
    public GetSubModulesByModuleQueryValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty().WithMessage("Module Id is required");
    }
}

public sealed class GetSubModulesByModuleQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetSubModulesByModuleQuery, Result<List<SubModuleListDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<SubModuleListDto>>> Handle(
        GetSubModulesByModuleQuery request,
        CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.SubModuleRepository.GetWhereIncludedAsync(x => x.ModuleBank.ModuleId == request.ModuleId && (!request.Type.HasValue || x.ModuleBank.Type == request.Type.Value), x => x.ModuleBank);


        var subModules =  query
            .OrderBy(x => x.ModuleBank.Type)
            .ThenBy(x => x.Difficulty ?? DifficultyType.None)
            .ThenBy(x => x.CreateDate)
            .Select(x => new SubModuleListDto
            {
                Id = x.Id,
                TaskDescription = x.TaskDescription,
                Difficulty = x.Difficulty,
                ModuleBankName = x.ModuleBank.Name,
                ModuleBankType = x.ModuleBank.Type,
                CreatedAt = x.CreateDate
            })
            .ToList();

        return Result<List<SubModuleListDto>>.Success(subModules);
    }
}