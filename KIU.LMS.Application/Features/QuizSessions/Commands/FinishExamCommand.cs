namespace KIU.LMS.Application.Features.QuizSessions.Commands;

public sealed record FinishExamCommand(string Id) : IRequest<Result>;


public class FinishExamCommandValidator : AbstractValidator<FinishExamCommand>
{
    public FinishExamCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}


public class FinishExamCommandHandler(IExamService _service, ICurrentUserService _current) : IRequestHandler<FinishExamCommand, Result>
{
    public async Task<Result> Handle(FinishExamCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.FinishExamAsync(request.Id);

        if (!result)
            return Result<string>.Failure("Can't end Session");

        return Result.Success("Session finished successfully");
    }
}
