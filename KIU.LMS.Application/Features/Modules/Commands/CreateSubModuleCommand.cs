using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;

namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record CreateBankModuleCommand(
        Guid ModuleId,
        string Name,
        SubModuleType Type) : IRequest<Result>;

public sealed class CreateSubModuleCommandValidator : AbstractValidator<CreateBankModuleCommand> 
{
    public CreateSubModuleCommandValidator() 
    {
        RuleFor(x => x.ModuleId)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class CreateSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CreateBankModuleCommand, Result>
{
    public async Task<Result> Handle(CreateBankModuleCommand request, CancellationToken cancellationToken)
    {
        var subModule = new ModuleBank(
            Guid.NewGuid(),
            request.Name,
            request.ModuleId,
            request.Type,
            _current.UserId);

        await _unitOfWork.ModuleBankRepository.AddAsync(subModule);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("module bank created successfully");
    }
}




// New Version

public sealed record CreateSubModuleCommand(
    Guid ModuleBankId,
    string? TaskDescription,
    string? CodeSolution,
    string? CodeGenerationPrompt,
    string? CodeGradingPrompt,
    string? Solution,
    DifficultyType? Difficulty
) : IRequest<Result<Guid>>;

public class CreateSubModulseCommandValidator : AbstractValidator<CreateSubModuleCommand>
{
    public CreateSubModulseCommandValidator()
    {
        RuleFor(x => x.ModuleBankId)
            .NotEmpty().WithMessage("ModuleBankId is required");

        RuleFor(x => x.TaskDescription)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.TaskDescription))
            .WithMessage("Task description cannot exceed 2000 characters");

        RuleFor(x => x.CodeSolution)
            .MaximumLength(4000).When(x => !string.IsNullOrEmpty(x.CodeSolution))
            .WithMessage("Code solution cannot exceed 4000 characters");

        RuleFor(x => x.CodeGenerationPrompt)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.CodeGenerationPrompt))
            .WithMessage("Code generation prompt cannot exceed 2000 characters");

        RuleFor(x => x.CodeGradingPrompt)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.CodeGradingPrompt))
            .WithMessage("Code grading prompt cannot exceed 2000 characters");

        RuleFor(x => x.Solution)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Solution))
            .WithMessage("Solution cannot exceed 2000 characters");

        RuleFor(x => x.Difficulty)
            .IsInEnum().When(x => x.Difficulty.HasValue)
            .WithMessage("Invalid difficulty level");
    }
}

public sealed class CreateSubModulesCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser
) : IRequestHandler<CreateSubModuleCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Result<Guid>> Handle(CreateSubModuleCommand request, CancellationToken cancellationToken)
    {
        // Verify ModuleBank exists
        var moduleBankExists = await _unitOfWork.ModuleBankRepository
            .ExistsAsync(x => x.Id == request.ModuleBankId);

        if (!moduleBankExists)
        {
            return Result<Guid>.Failure("ModuleBank not found");
        }

        // Get ModuleBank to check type
        var moduleBank = await _unitOfWork.ModuleBankRepository
            .SingleOrDefaultAsync(x => x.Id == request.ModuleBankId);

        // Validation based on ModuleBank type
        if (moduleBank.Type == SubModuleType.C2RS)
        {
            if (string.IsNullOrWhiteSpace(request.TaskDescription) ||
                string.IsNullOrWhiteSpace(request.CodeSolution) ||
                string.IsNullOrWhiteSpace(request.CodeGenerationPrompt) ||
                string.IsNullOrWhiteSpace(request.CodeGradingPrompt) ||
                !request.Difficulty.HasValue || request.Difficulty == DifficultyType.None)
            {
                return Result<Guid>.Failure("For C2RS type, all fields are required including difficulty");
            }
        }

        var subModuleId = Guid.NewGuid();
        var subModule = new SubModule(
            subModuleId,
            request.ModuleBankId,
            request.TaskDescription,
            request.CodeSolution,
            request.CodeGenerationPrompt,
            request.CodeGradingPrompt,
            request.Solution,
            request.Difficulty,
            _currentUser.UserId
        );

        await _unitOfWork.SubModuleRepository.AddAsync(subModule);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(subModuleId, "SubModule created successfully");
    }
}