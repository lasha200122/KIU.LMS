namespace KIU.LMS.Domain.Entities.SQL;

public class  QuestionBank : Aggregate
{
    public string Name { get; private set; } = null!;
    public Guid ModuleId { get; private set; }

    public virtual Module Module { get; private set; } = null!;
    private List<QuizBank> _quizBanks = new();
    public IReadOnlyCollection<QuizBank> QuizBanks => _quizBanks;


    public QuestionBank() { }

    public QuestionBank(
        Guid id,
        string name,
        Guid moduleId,
        Guid createUserId) : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        Name = name;
        ModuleId = moduleId;
        Validate(this);
    }

    public void Update(string name, Guid userId) 
    {
        Name = name;
        Update(userId, DateTimeOffset.UtcNow);
    }

    private void Validate(QuestionBank bank)
    {
        if (string.IsNullOrEmpty(bank.Name))
            throw new Exception("კითხვების ბანკის სახელი სავალდებულოა");
    }
}