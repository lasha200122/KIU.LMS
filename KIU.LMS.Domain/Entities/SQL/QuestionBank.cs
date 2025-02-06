namespace KIU.LMS.Domain.Entities.SQL;

public class QuestionBank : Aggregate
{
    public string Name { get; private set; } = null!;

    private List<QuizBank> _quizBanks = new();
    public IReadOnlyCollection<QuizBank> QuizBanks => _quizBanks;

    public QuestionBank() { }

    public QuestionBank(
        Guid id,
        string name,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        Name = name;
        Validate(this);
    }

    private void Validate(QuestionBank bank)
    {
        if (string.IsNullOrEmpty(bank.Name))
            throw new Exception("კითხვების ბანკის სახელი სავალდებულოა");
    }
}