namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record UpdateModuleBankCommand(
        Guid Id,
        string Name,
        SubModuleType Type) : IRequest<Result>;

public sealed class UpdateSubModuleCommandValidator : AbstractValidator<UpdateModuleBankCommand>
{
    public UpdateSubModuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class UpdateSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateModuleBankCommand, Result>
{
    public async Task<Result> Handle(UpdateModuleBankCommand request, CancellationToken cancellationToken)
    {
        var sub = await _unitOfWork.ModuleBankRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (sub is null)
            return Result.Failure("Can't find sub module");

        sub.UModuleBank(
            request.Name,
            request.Type,   
            _current.UserId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Sub module updated successfully");
    }
}

// New Version

public sealed record UpdateSubModuleCommand(
    Guid Id,
    string? TaskDescription,
    string? CodeSolution,
    string? CodeGenerationPrompt,
    string? CodeGradingPrompt,
    string? Solution,
    DifficultyType? Difficulty
) : IRequest<Result>;

public class UpdateSubModulesCommandValidator : AbstractValidator<UpdateSubModuleCommand>
{
    public UpdateSubModulesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("SubModule Id is required");

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

public sealed class UpdateSubModulesCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser
) : IRequestHandler<UpdateSubModuleCommand, Result>
{
    public async Task<Result> Handle(UpdateSubModuleCommand request, CancellationToken cancellationToken)
    {
        var subModule = await unitOfWork.SubModuleRepository
            .SingleOrDefaultAsync(
                predicate: x => x.Id == request.Id, x => x.ModuleBank
            );

        if (subModule == null)
        {
            return Result.Failure("SubModule not found");
        }

        // Validation based on ModuleBank type
        if (subModule.ModuleBank.Type == SubModuleType.C2RS)
        {
            if (string.IsNullOrWhiteSpace(request.TaskDescription) ||
                string.IsNullOrWhiteSpace(request.CodeSolution) ||
                string.IsNullOrWhiteSpace(request.CodeGenerationPrompt) ||
                string.IsNullOrWhiteSpace(request.CodeGradingPrompt) ||
                !request.Difficulty.HasValue || request.Difficulty == DifficultyType.None)
            {
                return Result.Failure("For C2RS type, all fields are required including difficulty");
            }
        }

        subModule.USub(
            request.TaskDescription,
            request.CodeSolution,
            request.CodeGenerationPrompt,
            request.CodeGradingPrompt,
            request.Solution,
            request.Difficulty,
            currentUser.UserId
        );

         unitOfWork.SubModuleRepository.Update(subModule);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("SubModule updated successfully");
    }
}