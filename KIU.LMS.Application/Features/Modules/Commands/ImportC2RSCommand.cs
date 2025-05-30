using Microsoft.AspNetCore.Http;

namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record ImportC2RSCommand(IFormFile File, Guid ModuleBankId) : IRequest<Result>;

public class ImportC2RSCommandValidator : AbstractValidator<ImportC2RSCommand>
{
    public ImportC2RSCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File cannot be null.")
            .Must(file => file.Length > 0)
            .WithMessage("File cannot be empty.");
    }
}

public sealed class ImportC2RSCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current, IExcelProcessor _excel) : IRequestHandler<ImportC2RSCommand, Result>
{
    public async Task<Result> Handle(ImportC2RSCommand request, CancellationToken cancellationToken)
    {
        var result = await _excel.ProcessTasksExcelFile(request.File);

        if (!result.IsValid)
        {
            return Result.Failure(string.Join(", ", result.Errors.Select(x => $"{x.ColumnName}, {x.RowNumber}, {x.ErrorMessage}")));
        }


        var modules = result.ValidTasks.Select(x => new SubModule(
            Guid.NewGuid(),
            request.ModuleBankId,
            x.TaskDescription,
            x.CodeSolution,
            x.CodeGenerationPrompt,
            x.CodeGradingPrompt,
            string.Empty,
            ParseDifficulty(x.Difficulty),
            _current.UserId
            )).ToList();


        await _unitOfWork.SubModuleRepository.AddRangeAsync(modules);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("C2RS data imported successfully.");
    }

    public static DifficultyType ParseDifficulty(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => DifficultyType.Easy,
            "medium" => DifficultyType.Medium,
            "hard" => DifficultyType.Hard,
            _ => DifficultyType.None
        };
    }
}