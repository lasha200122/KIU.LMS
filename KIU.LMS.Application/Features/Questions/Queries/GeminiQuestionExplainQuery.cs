using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GeminiQuestionExplainQuery(string Id) : IRequest<Result<string>>;

public sealed class GeminiQuestionExplainQueryHandler(IGeminiService _gemini, IMongoRepository<Question> _mongo) : IRequestHandler<GeminiQuestionExplainQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GeminiQuestionExplainQuery request, CancellationToken cancellationToken)
    {
        var question = await _mongo.FindOneAsync(x => x.Id == request.Id);

        if (question is null)
            return Result<string>.Failure("Can't find question");

        var correctOption = question.Options.FirstOrDefault(x => x.IsCorrect);
        var prompt = BuildPrompt(question, correctOption!);

        var response = await _gemini.GenerateContentAsync(prompt);
    
        return Result<string>.Success(response);
    }

    private static string BuildPrompt(Question question, Option correctOption)
    {
        return $@"You are a friendly and patient tutor helping a beginner student understand a question and its answer. The question and correct answer will be provided below.

Question: {question.Text}

Options:
{string.Join("\n", question.Options.Select(o => $"- {o.Text}"))}

Correct Answer: {correctOption.Text}

Please provide a beginner-friendly explanation following these guidelines:

1. Start with a simple confirmation of the correct answer
2. Break down the explanation into clear, easy-to-understand steps
3. Use everyday examples or analogies when possible to make the concept more relatable
4. If there are common misconceptions related to this topic, address them briefly
5. Explain why the other options are incorrect (if applicable)
6. End with a brief summary that reinforces the main point

Please avoid:
- Using complex technical terms without explanation
- Making assumptions about prior knowledge
- Providing overly complex explanations
- Using negative or discouraging language

Format your response in this structure:
""The correct answer is [answer].

Here's why this is correct:
[Step-by-step explanation]

Let's look at why the other options aren't correct:
[Brief explanation of incorrect options]

To remember this:
[Simple memory aid or key takeaway]""";
    }
}
