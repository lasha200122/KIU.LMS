using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Questions.Commands;

public sealed record DeleteGeneratedQuestionCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteGeneratedQuestionCommandHandler(
    IGeneratedQuestionRepository generatedQuestionRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteGeneratedQuestionCommand, Result>
{
    public async Task<Result> Handle(DeleteGeneratedQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await generatedQuestionRepository
            .SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (question is null)
            return Result.Failure("Generated question not found");

        // IBase-shi Remove
        generatedQuestionRepository.Remove(question);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Generated question deleted successfully");
    }
}