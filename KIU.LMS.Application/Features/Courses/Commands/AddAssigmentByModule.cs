using System.Data;
using System.Diagnostics;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddAssigmentByModulesCommand(
    Guid CourseId,
    Guid TopicId,
    Guid ModuleBankId,
    AssignmentType Type,
    bool IsPublic,
    bool AIGrader,
    bool FullScreen,
    bool IsTraining,
    Dictionary<DifficultyType, int> DifficultyDistribution,
    Guid CreateUserId) : IRequest<Result>;

public sealed class AddAssigmentByModulesCommandValidator : AbstractValidator<AddAssigmentByModulesCommand>
{
    public AddAssigmentByModulesCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty();
        RuleFor(x => x.TopicId)
            .NotEmpty();
        RuleFor(x => x.ModuleBankId)
            .NotEmpty();
        RuleFor(x => x.CreateUserId)
            .NotEmpty();

        RuleForEach(x => x.DifficultyDistribution)
            .Must(kv => Enum.IsDefined(typeof(DifficultyType), kv.Key))
            .WithMessage("Unknown difficulty type");

        RuleFor(x => x.DifficultyDistribution)
            .NotEmpty().WithMessage("Must specify at least one difficulty group");
        RuleFor(x => x.DifficultyDistribution.Values)
            .Must(vs => vs.All(n => n >= 0))
            .WithMessage("Value in difficulty distribution must be >= 0");
        RuleFor(x => x.DifficultyDistribution.Values)
            .Must(vs => vs.Sum() > 0)
            .WithMessage("Sum of assignments must be > 0");
    }
}

public sealed class AddAssigmentByModulesHandler(
    IAssignmentRepository assignmentRepository,
    IModuleBankRepository moduleBankRepository, 
    IValidator<AddAssigmentByModulesCommand> validator,
    IUnitOfWork unitOfWork 
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
        
        var moduleBank = await moduleBankRepository
            .AsQueryable()
            .Include(mb => mb.Module)
            .Include(mb => mb.SubModules)
            .FirstOrDefaultAsync(mb => mb.Id == request.ModuleBankId, cancellationToken);

        if (moduleBank is null)
            return Result.Failure("ModuleBank not found");

        if (moduleBank.Module.CourseId != request.CourseId)
            return Result.Failure("ModuleBank doesn't belong to the course");

        var available = moduleBank.SubModules
            .Where(sm => sm.Difficulty.HasValue)
            .GroupBy(sm => sm.Difficulty!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (difficulty, count) in request.DifficultyDistribution)
        {
            var found = available.GetValueOrDefault(difficulty)?.Count ?? 0;
            if (count > found)
                return Result.Failure($"Not enough {difficulty} tasks: Available {found}, requested {count}");
        }

        var random = new Random();
        var selectedSubmodules = new List<(SubModule SubModule, DifficultyType Difficulty)>();
        foreach (var (difficulty, count) in request.DifficultyDistribution)
        {
            if (count <= 0 || !available.ContainsKey(difficulty)) continue;
            var subs = available[difficulty]
                .OrderBy(_ => random.Next())
                .Take(count)
                .ToList();
            selectedSubmodules.AddRange(subs.Select(s => (s, difficulty)));
        }

        int order = await assignmentRepository
            .AsQueryable()
            .Where(a => a.TopicId == request.TopicId)
            .Select(a => (int?)a.Order)
            .MaxAsync(cancellationToken) ?? 0;
        order++; 

        var assignments = new List<Assignment>();
        foreach (var (sub, difficulty) in selectedSubmodules)
        {
            var score = ScoreMapping.GetValueOrDefault(difficulty, 0);

            var assignment = new Assignment(
                id: Guid.NewGuid(),
                courseId: request.CourseId,
                topicId: request.TopicId,
                type: request.Type,
                name: $"{moduleBank.Name} - {difficulty} #{order}",
                order: order++,
                startDateTime: null,
                endDateTime: null,
                score: score,
                problem: sub.TaskDescription,
                code: null,
                fileName: null,
                isPublic: request.IsPublic,
                aiGrader: request.AIGrader,
                solutionType: SolutionType.Code,
                promptId: null,
                fullScreen: request.FullScreen,
                runtimeAttempt: null,
                isTraining: request.IsTraining,
                promptText: sub.CodeGraidingPrompt,
                codeSolution: sub.CodeSolution,
                createUserId: request.CreateUserId
            );

            assignments.Add(assignment);
        }

        await assignmentRepository.AddRangeAsync(assignments, cancellationToken);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
