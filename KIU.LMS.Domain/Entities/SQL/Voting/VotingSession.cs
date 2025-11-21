public class VotingSession : Aggregate
{
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    
    private readonly List<VotingOption> _options = new();
    public IReadOnlyCollection<VotingOption> Options => _options.AsReadOnly();

    private VotingSession() { }

    public VotingSession(Guid id, Guid creatorId, string name, DateTimeOffset endTime)
        : base(id, DateTimeOffset.UtcNow, creatorId)
    {
        Name = name;
        IsActive = true;
        EndTime = endTime;
    }

    public void AddOption(Guid fileRecordId, Guid userId)
    {
        _options.Add(new VotingOption(Guid.NewGuid(), Id, fileRecordId, userId));
    }

    public void Close(Guid userId)
    {
        IsActive = false;
        Update(userId, DateTimeOffset.UtcNow);
    }
    
    public bool CanVote() => IsActive && DateTimeOffset.UtcNow <= EndTime;
}