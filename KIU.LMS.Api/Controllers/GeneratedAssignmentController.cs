namespace KIU.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneratedAssignmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public GeneratedAssignmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in request body and URL must match.");

        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? Ok(result) 
            : BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteAssigmnentCommand(id));

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result.Message);
    }
}