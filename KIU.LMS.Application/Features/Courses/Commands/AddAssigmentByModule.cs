using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddAssigmentByModulesCommand(
    Guid CourseId,
    Guid TopicId,
    AssignmentType Type,
    SolutionType SolutionType,
    bool IsPublic,
    bool AIGrader,
    bool FullScreen,
    bool IsTraining,
    int ValidationsCount,
    int? GenerationAttempt,
    DateTime? StartDateTime,
    DateTime? EndDateTime,
    BanksDto[] BanksDto) : IRequest<Result>;

public record BanksDto(
    Guid ModuleBankId,
    DifficultyType Difficulty,
    int Count);

public sealed class AddAssigmentByModulesCommandValidator : AbstractValidator<AddAssigmentByModulesCommand>
{
    public AddAssigmentByModulesCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty();

        RuleFor(x => x.TopicId)
            .NotEmpty();

        RuleFor(x => x.ValidationsCount)
            .LessThanOrEqualTo(3)
            .WithMessage("ValidationsCount must be <= 3");

        RuleFor(x => x.BanksDto)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one bank must be specified");

        RuleForEach(x => x.BanksDto).ChildRules(bank =>
        {
            bank.RuleFor(b => b.ModuleBankId)
                .NotEmpty()
                .WithMessage("ModuleBankId is required");

            bank.RuleFor(b => b.Count)
                .GreaterThan(0)
                .WithMessage("Count must be > 0");

            bank.RuleFor(b => b.Difficulty)
                .Must(d => Enum.IsDefined(typeof(DifficultyType), d))
                .WithMessage("Unknown difficulty type");
        });

        RuleFor(x => x.BanksDto.Sum(b => b.Count))
            .GreaterThan(0)
            .WithMessage("Total assignments count across all module banks must be > 0");
    }
}

public sealed class AddAssigmentByModulesHandler(
    IAssignmentRepository assignmentRepository,
    IModuleBankRepository moduleBankRepository,
    IValidator<AddAssigmentByModulesCommand> validator,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) : IRequestHandler<AddAssigmentByModulesCommand, Result>
{
    private static readonly Dictionary<DifficultyType, decimal> ScoreMapping = new()
    {
        { DifficultyType.Easy, 4m },
        { DifficultyType.Medium, 8m },
        { DifficultyType.Hard, 12m }
    };

    public async Task<Result> Handle(AddAssigmentByModulesCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));

        var order = await assignmentRepository
            .AsQueryable()
            .Where(a => a.TopicId == request.TopicId)
            .Select(a => (int?)a.Order)
            .MaxAsync(cancellationToken) ?? 0;
        
        order++;

        var assignments = new List<Assignment>();
        var random = new Random();

        foreach (var bank in request.BanksDto)
        {
            var moduleBank = await moduleBankRepository
                .AsQueryable()
                .Include(mb => mb.Module)
                .Include(mb => mb.SubModules)
                .FirstOrDefaultAsync(mb => mb.Id == bank.ModuleBankId, cancellationToken);

            if (moduleBank is null)
                return Result.Failure($"ModuleBank {bank.ModuleBankId} not found");

            if (moduleBank.Module.CourseId != request.CourseId)
                return Result.Failure($"ModuleBank {bank.ModuleBankId} doesn't belong to the course");

            var available = moduleBank.SubModules
                .Where(sm => sm.Difficulty.HasValue)
                .GroupBy(sm => sm.Difficulty!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var found = available.GetValueOrDefault(bank.Difficulty)?.Count ?? 0;
            if (bank.Count > found)
                return Result.Failure(
                    $"Not enough {bank.Difficulty} tasks in bank {moduleBank.Name}. Available {found}, requested {bank.Count}");

            var selectedSubmodules = available[bank.Difficulty]
                .OrderBy(_ => random.Next())
                .Take(bank.Count)
                .ToList();
            
            assignments.AddRange(
                from sub in selectedSubmodules
                let score = ScoreMapping.GetValueOrDefault(bank.Difficulty, 0)
                let isIpeqType = request.Type == AssignmentType.IPEQ
                let code = isIpeqType ? sub.CodeSolution : null
                let codeSolution = request.SolutionType == SolutionType.Code ? sub.CodeSolution : null
                let runtimeAttempt = isIpeqType ? request.GenerationAttempt : null
                select new Assignment(
                    id: Guid.NewGuid(), 
                    courseId: request.CourseId, 
                    topicId: request.TopicId,
                    type: request.Type, 
                    name: $"{moduleBank.Name} - {bank.Difficulty} #{order}", 
                    order: order++,
                    startDateTime: request.StartDateTime, 
                    endDateTime: request.EndDateTime, 
                    score: score, 
                    problem: sub.TaskDescription, 
                    code: code,
                    fileName: null, 
                    isPublic: request.IsPublic, 
                    aiGrader: request.AIGrader,
                    solutionType: request.SolutionType, 
                    promptId: null, 
                    fullScreen: request.FullScreen,
                    runtimeAttempt: runtimeAttempt, 
                    isTraining: request.IsTraining, 
                    promptText: sub.CodeGraidingPrompt,
                    codeSolution: codeSolution, 
                    validationsCount: request.ValidationsCount,
                    createUserId: currentUserService.UserId));
        }

        await assignmentRepository.AddRangeAsync(assignments, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
