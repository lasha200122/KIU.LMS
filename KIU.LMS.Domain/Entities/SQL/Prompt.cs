namespace KIU.LMS.Domain.Entities.SQL;

public class Prompt : Aggregate
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public virtual List<Assignment> Assignments { get; set; } = null!;
    public Prompt() { }

    public Prompt(
        Guid id, string titile, string value, Guid userId) : base(id, DateTimeOffset.UtcNow, userId)
    { Value = value;  Title = titile; }
}
