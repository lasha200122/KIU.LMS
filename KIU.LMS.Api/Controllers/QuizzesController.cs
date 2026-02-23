namespace KIU.LMS.Api.Controllers;

[Route("api/quizzes")]
[Authorize]
public class QuizzesController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IResult> AddQuiz([FromBody] AddQuizCommand request)
    {
        return await Handle(request);
    }

    [HttpPut("{id}/seb-settings")]
    public async Task<IResult> UpdateSebSettings(Guid id, [FromBody] UpdateQuizSebSettingsRequest request)
    {
        var command = new UpdateQuizSebSettingsCommand(id, request.RequiresSafeExamBrowser, request.RegenerateConfig);
        return await Handle(command);
    }

    [HttpGet("{id}/seb-config")]
    public async Task<IActionResult> DownloadSebConfig(Guid id)
    {
        var query = new GenerateSebConfigQuery(id);
        var result = await sender.Send(query);

        if (!result.IsSuccess || result.Data == null)
            return BadRequest(new { message = result.Message });

        return File(result.Data.FileContent, result.Data.ContentType, result.Data.FileName);
    }
}

public record UpdateQuizSebSettingsRequest(bool RequiresSafeExamBrowser, bool RegenerateConfig = false);
