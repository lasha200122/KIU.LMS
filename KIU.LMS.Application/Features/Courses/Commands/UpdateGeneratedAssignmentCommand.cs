using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateGeneratedAssignmentCommand(
    Guid Id,
    string Title,
    int Count,
    List<string> Models,
    string TaskContent,
    DifficultyType Difficulty,
    string Prompt,
    GeneratingStatus Status
) : IRequest<Result>;

public class UpdateGeneratedAssignmentCommandValidator 
    : AbstractValidator<UpdateGeneratedAssignmentCommand>
{
    public UpdateGeneratedAssignmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Count)
            .GreaterThan(0);

        RuleFor(x => x.Models)
            .NotNull()
            .Must(m => m.Any())
            .WithMessage("At least one model is required.");

        RuleFor(x => x.TaskContent)
            .NotEmpty();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.Prompt)
            .NotEmpty(); 

        RuleFor(x => x.Status)
            .IsInEnum();
        
    }
}

public sealed class UpdateGeneratedAssignmentHandler
    : IRequestHandler<UpdateGeneratedAssignmentCommand, Result>
{
    private readonly IGeneratedAssignmentRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGeneratedAssignmentHandler(
        IGeneratedAssignmentRepository repo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateGeneratedAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repo.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (assignment == null)
            return Result.Failure("Assignment not found.");
        
        assignment.Update(
            title: request.Title,
            taskContent: request.TaskContent,
            count: request.Count,
            difficulty: request.Difficulty,
            prompt: request.Prompt,
            models: request.Models
        );


        assignment.UpdateStatus(request.Status);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
