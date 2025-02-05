namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record IpeqExecuteCommand(string Prompt) : IRequest<Result<string?>>;

public sealed class IpeqExecuteCommandHandler(IGeminiService _gemini) : IRequestHandler<IpeqExecuteCommand, Result<string?>>
{
    public async Task<Result<string?>> Handle(IpeqExecuteCommand request, CancellationToken cancellationToken)
    {
        return await _gemini.GenerateContentAsync(request.Prompt);
    }
}
