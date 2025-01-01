namespace KIU.LMS.Domain.Common.Interfaces.Entities;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    string Id { get; set; }

    [BsonElement("created_at")]
    [BsonRepresentation(BsonType.DateTime)]
    DateTimeOffset CreatedAt { get; }

    [BsonElement("updated_at")]
    [BsonRepresentation(BsonType.DateTime)]
    DateTimeOffset? UpdatedAt { get; }

    [BsonElement("is_deleted")]
    bool IsDeleted { get; }
}