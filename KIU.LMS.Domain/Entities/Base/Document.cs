namespace KIU.LMS.Domain.Entities.Base;

public abstract class Document : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("created_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreatedAt { get; private set; }

    [BsonElement("updated_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? UpdatedAt { get; private set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; private set; }

    protected Document()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Update()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        Update();
    }
}