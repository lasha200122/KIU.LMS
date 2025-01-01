namespace KIU.LMS.Domain.Entities.SQL;

public class ExamQuestion : Aggregate
{
    public Guid ExamId { get; private set; }
    public Guid QuestionBankId { get; private set; }
    public int NumberOfQuestions { get; private set; }

    public virtual Exam Exam { get; private set; } = null!;
    public virtual QuestionBank QuestionBank { get; private set; } = null!;

    public ExamQuestion() { }

    public ExamQuestion(
        Guid id,
        Guid examId,
        Guid questionBankId,
        int numberOfQuestions,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        ExamId = examId;
        QuestionBankId = questionBankId;
        NumberOfQuestions = numberOfQuestions;
        Validate(this);
    }

    private void Validate(ExamQuestion question)
    {
        if (question.ExamId == default)
            throw new Exception("გამოცდის ID სავალდებულოა");
        if (question.QuestionBankId == default)
            throw new Exception("კითხვების ბანკის ID სავალდებულოა");
        if (question.NumberOfQuestions <= 0)
            throw new Exception("კითხვების რაოდენობა უნდა იყოს დადებითი რიცხვი");
    }
}