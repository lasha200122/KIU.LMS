namespace KIU.LMS.Domain.Entities.SQL;

public class QuizBank : Aggregate
{
    public Guid QuizId { get; private set; }
    public Guid QuestionBankId { get; private set; }
    public int Amount { get; private set; }

    public virtual QuestionBank QuestionBank { get; private set; } = null!;
    public virtual Quiz Quiz { get; private set; } = null!;
    public QuizBank() {}

    public QuizBank(
        Guid id, Guid quizId, Guid questionBankId, int amount, Guid userId) : base(id, DateTimeOffset.UtcNow, userId)
    {
        QuizId = quizId;
        QuestionBankId = questionBankId;
        Amount = amount;
    }
}
