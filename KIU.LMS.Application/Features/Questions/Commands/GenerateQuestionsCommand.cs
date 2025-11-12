using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Questions.Commands;

public record GenerateQuestionsCommand(
    string Task,
    int Count,
    DifficultyType Difficulty,
    List<string> Models) : IRequest<Result>;

public sealed class GenerateQuestionsHandler(
    IGeneratedAssignmentRepository generatedAssignmentRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService userService) : IRequestHandler<GenerateQuestionsCommand, Result>
{
    public async Task<Result> Handle(GenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var generatedAssignmentCount = await generatedAssignmentRepository.CountAsync(cancellationToken);

        GeneratedAssignment generatedAssignment = new(
            Guid.NewGuid(), userService.UserId,
            $"Assigment {generatedAssignmentCount}", request.Task,
            request.Count, request.Difficulty, request.Models);
        
        await generatedAssignmentRepository.AddAsync(generatedAssignment);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
