public class VotingOption : Aggregate
{
    public Guid SessionId { get; private set; }
    public Guid FileRecordId { get; private set; }
    
    public virtual FileRecord FileRecord { get; private set; }

    private VotingOption() { }
    
    public VotingOption(Guid id, Guid sessionId, Guid fileRecordId, Guid createUserId)
    {
        Id = id;
        SessionId = sessionId;
        FileRecordId = fileRecordId;
    }
}