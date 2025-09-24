namespace KIU.LMS.Domain.Entities.SQL;

public class FileRecord : Aggregate
{
    public FileRecord(
        Guid id,
        string objectId,
        string objectType,
        string fileName,
        string? filePath,
        string? contentType,
        long fileSize,
        DateTime uploadDate) : base(id, DateTimeOffset.UtcNow, id) 
    {
        ObjectId = objectId;
        ObjectType = objectType;
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        UploadDate = uploadDate;
    }

    public string ObjectId { get; set; } = string.Empty;

    public string ObjectType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string? FilePath { get; set; }

    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }
}
