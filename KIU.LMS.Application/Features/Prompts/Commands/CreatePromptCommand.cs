namespace KIU.LMS.Application.Features.Prompts.Commands;

public sealed record CreatePromptCommand(string Title, string Value) : IRequest<Result>;

public sealed class CreatePromptCommandValidator : AbstractValidator<CreatePromptCommand> 
{
    public CreatePromptCommandValidator() 
    {
        RuleFor(x => x.Title)
            .NotNull();

        RuleFor(x => x.Value)
            .NotNull();
    }
}

public sealed class CreatePromptCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CreatePromptCommand, Result>
{
    public async Task<Result> Handle(CreatePromptCommand request, CancellationToken cancellationToken)
    {
        var prompt = new Prompt(
            Guid.NewGuid(),
            request.Title,
            request.Value,
            _current.UserId);

        await _unitOfWork.PromptRepository.AddAsync(prompt);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Prompt created successfully");
    }
}
