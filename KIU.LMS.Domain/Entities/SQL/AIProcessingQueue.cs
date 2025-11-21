namespace KIU.LMS.Domain.Entities.SQL;

public class AIProcessingQueue : Aggregate
{
    public Guid TargetId { get; set; }
    public AIProcessingType Type { get; private set; }
    public AIProcessingStatus Status { get; private set; }
    public string? MetaData { get; private set; }
    public string Result { get; private set; } = string.Empty;
    
    public AIProcessingQueue(Guid id, Guid createUserId,
        Guid targetId, AIProcessingType type, string? metaData)
        : base(id, DateTimeOffset.UtcNow, createUserId)
    {
           TargetId = targetId;
           Type = type;
           MetaData = metaData;
           Status = AIProcessingStatus.Pending;
    }

    public void Start()
    {
        Status = AIProcessingStatus.InProgress;
        Touch();
    }

    public void MarkCompleted(string jsonResult)
    {
        Status = AIProcessingStatus.Completed;
        Result = jsonResult;
        Touch();
    }

    public void MarkFailed(string errorMessage)
    {
        Status = AIProcessingStatus.Failed;
        Result = errorMessage;
        Touch();
    }

    public void MarkToRetry()
    {
        Status = AIProcessingStatus.InProgress;
        Touch();
    }
    
    private void Touch()
    {
        Update(CreateUserId, DateTimeOffset.UtcNow);
    }
    
}


public enum AIProcessingType
{
    None = 0,
    MCQ = 1,
    C2RS = 2,
    IPEQ = 3,
    Grading = 4
}

public enum AIProcessingStatus
{
    None = 0,
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4
}

public sealed record AIProcessingResult(
    bool Success,
    string ResultJson, 
    string? ErrorMessage = null
);

//TODO : davamato endpointi AIProcessingebis misagebad 