namespace KIU.LMS.Domain.Entities.SQL;

public class Quiz : Aggregate
{
    public string Name { get; private set; } = string.Empty;
}
