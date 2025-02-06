namespace KIU.LMS.Application.Features.QuizSessions.Commands;

public sealed record StartSessionCommand(Guid Id) : IRequest<Result<string>>;


public class StartSessionCommandValidator : AbstractValidator<StartSessionCommand> 
{
    public StartSessionCommandValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}


public class StartSessionCommandHandler(IExamService _service, ICurrentUserService _current) : IRequestHandler<StartSessionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.StartExamAsync(_current.UserId, request.Id);

        if (result is null)
            return Result<string>.Failure("Can't start Session");

        return Result<string>.Success(result.Id);
    }
}
