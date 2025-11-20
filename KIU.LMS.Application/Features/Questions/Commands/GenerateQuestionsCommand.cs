using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Questions.Commands;

public record GenerateQuestionsCommand(
    string Task,
    int Count,
    DifficultyType Difficulty,
    string Prompt,
    GeneratedAssignmentType AssignmentType,
    List<string> Models) : IRequest<Result>;

public sealed class GenerateQuestionsHandler(
    IGeneratedAssignmentRepository generatedAssignmentRepository,
    IAIProcessingQueueRepository aiProcessingQueueRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService userService) : IRequestHandler<GenerateQuestionsCommand, Result>
{
    public async Task<Result> Handle(GenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var generatedAssignmentCount = await generatedAssignmentRepository.CountAsync(cancellationToken);

        GeneratedAssignment generatedAssignment = new(
            Guid.NewGuid(), userService.UserId,
            $"Assigment {generatedAssignmentCount}", request.Task, 
            request.Count, request.Difficulty, request.AssignmentType,
            request.Prompt, request.Models);

        var type = request.AssignmentType switch
        {
            GeneratedAssignmentType.MCQ => AIProcessingType.MCQ,
            GeneratedAssignmentType.C2RS => AIProcessingType.C2RS,
            GeneratedAssignmentType.IPEQ => AIProcessingType.IPEQ,
            _ => AIProcessingType.None
        };

        await aiProcessingQueueRepository.AddAsync(new AIProcessingQueue(
            Guid.NewGuid(), userService.UserId,
            generatedAssignment.Id, type, request.Task
        ));
        
        await generatedAssignmentRepository.AddAsync(generatedAssignment);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
