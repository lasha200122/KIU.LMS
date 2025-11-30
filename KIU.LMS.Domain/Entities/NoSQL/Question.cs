using KIU.LMS.Domain.Entities.NoSQL;

[BsonCollection("questions")]
public class Question : Document
{
    public string QuestionBankId { get; private set; } = string.Empty;
    public QuestionType Type { get; private set; }
    
    // MCQ fields
    public string? Text { get; private set; }
    public string? ExplanationCorrectAnswer { get; private set; }
    public string? ExplanationIncorrectAnswer { get; private set; }
    public List<Option>? Options { get; private set; }
    
    // IPEQ/C2RS fields
    public string? TaskDescription { get; private set; }
    public string? ReferenceSolution { get; private set; }
    public string? CodeGenerationPrompt { get; private set; }
    public string? CodeGradingPrompt { get; private set; }
    
    public Question() {}
    
    public Question(
        Guid questionBankId,
        string text,
        List<Option> options,
        string explanationCorrectAnswer,
        string explanationIncorrectAnswer) 
    {
        QuestionBankId = questionBankId.ToString();
        Type = QuestionType.Multiple;
        Text = text;
        Options = options;
        ExplanationCorrectAnswer = explanationCorrectAnswer;
        ExplanationIncorrectAnswer = explanationIncorrectAnswer;
    }
    
    public Question(
        Guid questionBankId,
        QuestionType type, 
        string taskDescription,
        string referenceSolution,
        string codeGenerationPrompt,
        string codeGradingPrompt)
    {
        QuestionBankId = questionBankId.ToString();
        Type = type;
        TaskDescription = taskDescription;
        ReferenceSolution = referenceSolution;
        CodeGenerationPrompt = codeGenerationPrompt;
        CodeGradingPrompt = codeGradingPrompt;
    }
}