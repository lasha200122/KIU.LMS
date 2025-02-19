namespace KIU.LMS.Application.Features.Prompts.Commands;

public sealed record DeletePromptCommand(Guid Id) : IRequest<Result>;

public sealed class DeletePromptCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeletePromptCommand, Result>
{
    public async Task<Result> Handle(DeletePromptCommand request, CancellationToken cancellationToken)
    {
        var prompt = await _unitOfWork.PromptRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (prompt is null)
            return Result.Failure("Can't find prompt");

        prompt.Delete(_current.UserId, DateTimeOffset.UtcNow);

        _unitOfWork.PromptRepository.Update(prompt);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Prompt deleted successfully");
    }
}
