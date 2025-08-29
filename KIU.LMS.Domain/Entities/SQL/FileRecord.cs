using System.ComponentModel.DataAnnotations;

namespace KIU.LMS.Domain.Entities.SQL;

public class FileRecord
{
    public Guid Id { get; set; }

    [Required]
    public string ObjectId { get; set; }

    [Required]
    public string ObjectType { get; set; }

    [Required]
    public string FileName { get; set; }

    [Required]
    public string FilePath { get; set; }

    public string ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }
}
