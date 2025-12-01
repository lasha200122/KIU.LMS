namespace KIU.LMS.Application.Features.QuizSessions.Commands;

public sealed record SubmitAnswerCommand(
    string SessionId,
    string QuestionId,
    List<string>? Options,
    string? StudentCode = "",
    string? StudentPrompt = ""
) : IRequest<Result>;

public sealed class SubmitAnswerCommandValidator : AbstractValidator<SubmitAnswerCommand> 
{
    public SubmitAnswerCommandValidator() 
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
    
public sealed class SubmitAnswerCommandHandler(IExamService _service, ICurrentUserService _current) 
    : IRequestHandler<SubmitAnswerCommand, Result>
{
    public async Task<Result> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.SubmitAnswerAsync(
            request.SessionId, 
            request.QuestionId, 
            request.Options,
            request.StudentCode,      
            request.StudentPrompt);   

        if (!result)
            return Result.Failure("Can't submit answer");

        return Result.Success("Answer submitted successfully");
    }
}