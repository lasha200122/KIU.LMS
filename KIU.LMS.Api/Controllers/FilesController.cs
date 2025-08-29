using System.IO;
using KIU.LMS.Domain.Common.Interfaces.Services;
using KIU.LMS.Domain.Entities.SQL;

namespace KIU.LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileRecord>> UploadFile(
        [FromForm] string objectId,
        [FromForm] string objectType,
        [FromForm] IFormFile file)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(objectId) || string.IsNullOrWhiteSpace(objectType))
            {
                return BadRequest("ObjectId and ObjectType are required");
            }

            var fileRecord = await _fileService.UploadFileAsync(objectId, objectType, file);
            return Ok(fileRecord);
        }
        catch (Exception ex)
        {
            return BadRequest($"Upload failed: {ex.Message}");
        }
    }

    [HttpGet("list/{objectId}/{objectType}")]
    public async Task<ActionResult<List<FileRecord>>> GetFiles(string objectId, string objectType)
    {
        var files = await _fileService.GetFilesByObjectAsync(objectId, objectType);
        return Ok(files);
    }

    [HttpGet("download/{fileId}")]
    public async Task<ActionResult> DownloadFile(Guid fileId)
    {
        try
        {
            var (fileData, fileName, contentType) = await _fileService.DownloadFileAsync(fileId);
            return File(fileData, contentType ?? "application/octet-stream", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found");
        }
    }

    [HttpDelete("delete/{fileId}")]
    public async Task<ActionResult> DeleteFile(Guid fileId)
    {
        var success = await _fileService.DeleteFileAsync(fileId);
        if (success)
            return Ok(new { message = "File deleted successfully" });
        else
            return NotFound("File not found");
    }

    [HttpGet("info/{fileId}")]
    public async Task<ActionResult<FileRecord>> GetFileInfo(Guid fileId)
    {
        var fileRecord = await _fileService.GetFileByIdAsync(fileId);
        if (fileRecord == null)
            return NotFound("File not found");

        return Ok(fileRecord);
    }
}