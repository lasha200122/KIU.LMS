namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record IpeqExecuteCommand(string Prompt) : IRequest<Result<string?>>;

public sealed class IpeqExecuteCommandHandler(IGeminiService gemini) : IRequestHandler<IpeqExecuteCommand, Result<string?>>
{
    public async Task<Result<string?>> Handle(IpeqExecuteCommand request, CancellationToken cancellationToken)
    {
        return await gemini.GenerateContentAsync(request.Prompt);
    }
}
