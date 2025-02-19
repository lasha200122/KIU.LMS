namespace KIU.LMS.Application.Features.Prompts.Commands;

public sealed record UpdatePromptCommand(Guid Id, string Title, string Value) : IRequest<Result>;

public sealed class UpdatePromptCommandValidator : AbstractValidator<UpdatePromptCommand>
{
    public UpdatePromptCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();

        RuleFor(x => x.Value)
            .NotNull();

        RuleFor(x => x.Id)
            .NotNull();
    }
}

public sealed class UpdatePromptCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdatePromptCommand, Result>
{
    public async Task<Result> Handle(UpdatePromptCommand request, CancellationToken cancellationToken)
    {
        var prompt = await _unitOfWork.PromptRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (prompt is null)
            return Result.Failure("Can't find prompt");

        prompt.Update(
            request.Title,
            request.Value,
            _current.UserId);

        _unitOfWork.PromptRepository.Update(prompt);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Prompt updated successfully");
    }
}
