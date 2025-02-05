namespace KIU.LMS.Api.Controllers;

[Route("api/excel")]
public class ExcelController(ISender _sender) : ApiController(_sender)
{
    [HttpGet("student-registration")]
    public async Task<IResult> StudentRegistrationTemplate() 
    {
        return await HandleFile(new RegisterStudentsTemplateQuery(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "student-template.xlsx");
    }

    [HttpGet("questions-registration")]
    public async Task<IResult> AddQuestionsTemplate()
    {
        return await HandleFile(new AddQuestionsTemplateQuery(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "questions-template.xlsx");
    }
}
