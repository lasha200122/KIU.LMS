namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IFileService
{
    Task<FileRecord> UploadFileAsync(string objectId, string objectType, IFormFile file);
    Task<List<FileRecord>> GetFilesByObjectAsync(string objectId, string objectType);
    Task<FileRecord> GetFileByIdAsync(Guid fileId);
    Task<bool> DeleteFileAsync(Guid fileId);
    Task<(byte[] fileData, string fileName, string contentType)> DownloadFileAsync(Guid fileId);
}