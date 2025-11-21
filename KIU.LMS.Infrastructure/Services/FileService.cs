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
        _uploadPath = "C:\\inetpub\\files";

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

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        var id = Guid.NewGuid();

        var fileName = file.FileName;
        var fileExtension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var uniqueFileName = $"{fileNameWithoutExtension}_{id}{fileExtension}";

        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        var fileRecord = new FileRecord(
            id,
            objectId,
            objectType,
            fileName,
            filePath,
            file.ContentType,
            file.Length,
            DateTime.UtcNow);

        _context.FileRecords.Add(fileRecord);
        await _context.SaveChangesAsync();

        return fileRecord;
    }

    public async Task<FileRecord> UploadFileAsync(string objectId, string objectType, IFormFile file, string fileName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);

        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("File must have extension");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
        if (!allowedExtensions.Contains(extension.ToLower()))
            throw new ArgumentException($"File type {extension} not supported");

        var id = Guid.NewGuid();
        var newFileName = $"{fileName}{extension}";
        var filePath = Path.Combine(_uploadPath, newFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var fileRecord = new FileRecord(
            id,
            objectId,
            objectType,
            newFileName,
            filePath,
            file.ContentType,
            file.Length,
            DateTime.UtcNow);

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

    public async Task<List<FileRecordDTO>> GetAllFiles()
    {
        return await _context.FileRecords
            .Select(x => new FileRecordDTO(x.Id, x.ObjectId, x.FileName))
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

