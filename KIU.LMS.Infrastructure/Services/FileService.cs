using KIU.LMS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly LmsDbContext _context;
    private readonly string _uploadPath;

    public FileService(LmsDbContext context)
    {
        _context = context;
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<FileRecord> UploadFileAsync(string objectId, string objectType, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        // Create directory structure: Uploads/{ObjectType}/{ObjectId}
        var objectDirectory = Path.Combine(_uploadPath, objectType, objectId);
        if (!Directory.Exists(objectDirectory))
        {
            Directory.CreateDirectory(objectDirectory);
        }

        // Generate unique filename to avoid conflicts
        var fileName = file.FileName;
        var fileExtension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";

        var filePath = Path.Combine(objectDirectory, uniqueFileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save file record to database
        var fileRecord = new FileRecord
        {
            ObjectId = objectId,
            ObjectType = objectType,
            FileName = fileName,
            FilePath = filePath,
            ContentType = file.ContentType,
            FileSize = file.Length,
            UploadDate = DateTime.UtcNow
        };

        _context.FileRecords.Add(fileRecord);
        await _context.SaveChangesAsync();

        return fileRecord;
    }

    public async Task<List<FileRecord>> GetFilesByObjectAsync(string objectId, string objectType)
    {
        return await _context.FileRecords
            .Where(f => f.ObjectId == objectId && f.ObjectType == objectType)
            .OrderByDescending(f => f.UploadDate)
            .ToListAsync();
    }

    public async Task<FileRecord> GetFileByIdAsync(Guid fileId)
    {
        return await _context.FileRecords.FindAsync(fileId);
    }

    public async Task<bool> DeleteFileAsync(Guid fileId)
    {
        var fileRecord = await _context.FileRecords.FindAsync(fileId);
        if (fileRecord == null)
            return false;

        // Delete physical file
        if (File.Exists(fileRecord.FilePath))
        {
            File.Delete(fileRecord.FilePath);
        }

        // Delete database record
        _context.FileRecords.Remove(fileRecord);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(byte[] fileData, string fileName, string contentType)> DownloadFileAsync(Guid fileId)
    {
        var fileRecord = await _context.FileRecords.FindAsync(fileId);
        if (fileRecord == null || !File.Exists(fileRecord.FilePath))
            throw new FileNotFoundException("File not found");

        var fileData = await File.ReadAllBytesAsync(fileRecord.FilePath);
        return (fileData, fileRecord.FileName, fileRecord.ContentType);
    }
}