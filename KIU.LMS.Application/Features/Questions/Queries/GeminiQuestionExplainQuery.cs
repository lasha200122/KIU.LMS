using Anthropic.SDK.Messaging;
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GeminiQuestionExplainQuery(string Id) : IRequest<Result<string>>;

public sealed class GeminiQuestionExplainQueryHandler(IClaudeService _gemini, IMongoRepository<Question> _mongo) : IRequestHandler<GeminiQuestionExplainQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GeminiQuestionExplainQuery request, CancellationToken cancellationToken)
    {
        var question = await _mongo.FindOneAsync(x => x.Id == request.Id);

        if (question is null)
            return Result<string>.Failure("Can't find question");

        var correctOption = question.Options.FirstOrDefault(x => x.IsCorrect);
        var prompt = BuildPrompt(question, correctOption!);

        var response = await _gemini.GenerateContentAsync(prompt.Messages, prompt.SystemMessages);
    
        return Result<string>.Success(response);
    }

    private static (List<SystemMessage> SystemMessages, List<Anthropic.SDK.Messaging.Message> Messages) BuildPrompt(Question question, Option correctOption)
    {
        var systemMessages = new List<SystemMessage>
    {
        new SystemMessage(
            "You are a friendly tutor who explains answers in a clear, simple way. " +
            "Always use examples, explain why wrong answers are incorrect, and provide a simple memory aid.")
    };

        var messages = new List<Anthropic.SDK.Messaging.Message>
    {
        new Anthropic.SDK.Messaging.Message(RoleType.User,
            $"Question: {question.Text}\n\n" +
            $"Options:\n{string.Join("\n", question.Options.Select(o => $"- {o.Text}"))}\n\n" +
            $"Correct Answer: {correctOption.Text}\n\n" +
            "Explain why this is correct, why others are wrong, and give a simple example.")
    };

        return (systemMessages, messages);
    }
}
