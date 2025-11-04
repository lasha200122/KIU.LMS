namespace KIU.LMS.Domain.Entities.SQL;

public class GeneratedQuestion : Aggregate
{
    public Guid GeneratedAssignmentId { get; private set; }
    public string QuestionText { get; private set; }
    public string OptionA { get; private set; } // Always correct
    public string OptionB { get; private set; }
    public string OptionC { get; private set; }
    public string OptionD { get; private set; }

    public string ExplanationCorrect { get; private set; }
    public string ExplanationIncorrect { get; private set; }
    
    public GeneratedAssignment Assignment { get; private set; } = null!;

    public GeneratedQuestion(
        Guid generatedAssignmentId, string questionText,
        string optionA, string optionB, string optionC, string optionD,
        string explanationCorrect, string explanationIncorrect)
    {
        GeneratedAssignmentId =  generatedAssignmentId;
        QuestionText = questionText;
        OptionA = optionA;
        OptionB = optionB;
        OptionC = optionC;
        OptionD = optionD;
        ExplanationCorrect = explanationCorrect;
        ExplanationIncorrect = explanationIncorrect;
        CreateDate = DateTimeOffset.UtcNow;
    }
}

