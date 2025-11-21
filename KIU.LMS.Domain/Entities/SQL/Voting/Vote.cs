public class Vote : Aggregate
{
    public Guid SessionId { get; private set; }
    public Guid OptionId { get; private set; }
    public Guid UserId { get; private set; }
    
    private Vote() { }

    public Vote(Guid id, Guid sessionId, Guid optionId, Guid userId)
        : base(id, DateTimeOffset.UtcNow, userId)
    {
        SessionId = sessionId;
        OptionId = optionId;
        UserId = userId;
    }
}