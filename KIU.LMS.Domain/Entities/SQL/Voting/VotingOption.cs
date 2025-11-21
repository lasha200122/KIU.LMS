public class VotingOption : Aggregate
{
    public Guid SessionId { get; private set; }
    public Guid FileRecordId { get; private set; }
    
    public virtual FileRecord FileRecord { get; private set; }
    
    private readonly List<Vote> _votes = new();
    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly(); 
    private VotingOption() { }
    
    public VotingOption(Guid id, Guid sessionId, Guid fileRecordId, Guid createUserId) 
        : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        Id = id;
        SessionId = sessionId;
        FileRecordId = fileRecordId;
    }
}