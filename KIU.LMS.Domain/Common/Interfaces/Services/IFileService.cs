namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IFileService
{
    public Task<FileRecord> UploadFileAsync(string objectId, string objectType, IFormFile file);
    Task<FileRecord> UploadFileAsync(string objectId, string objectType, IFormFile file, string fileName);
    Task<List<FileRecord>> GetFilesByObjectAsync(string objectId, string objectType);
    Task<FileRecord> GetFileByIdAsync(Guid fileId);
    Task<bool> DeleteFileAsync(Guid fileId);
    Task<(byte[] fileData, string fileName, string contentType)> DownloadFileAsync(Guid fileId);
    Task<List<FileRecordDTO>> GetAllFiles();
}

public sealed record FileRecordDTO(Guid Id, string ObjectId, string FileName);