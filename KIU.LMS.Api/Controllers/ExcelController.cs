using DocumentFormat.OpenXml.Office2010.Excel;

namespace KIU.LMS.Api.Controllers;

[Route("api/excel")]
[Authorize]
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


    [HttpGet("course-registration")]
    public async Task<IResult> AddCourseTemplate()
    {
        return await HandleFile(new AddCourseStudentsTemplateQuery(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "course-template.xlsx");
    }

    [HttpGet("quiz/{id}/results")]
    public async Task<IResult> GetQuizResults(Guid id)
    {
        return await HandleFile(new GetQuizResultsQuery(id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "quiz-result.xlsx");
    }

    [HttpGet("finalists")]
    public async Task<IResult> GetFinalists([FromQuery] GeneretaFinalistExcel request) 
    {
        return await HandleFile(request, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "finalists.xlsx");
    }


    [HttpGet("c2rs")]
    public async Task<IResult> GetC2RsTemplate()
    {
        return await HandleFile(new C2RSTemplateQuery(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "c2rs-tempalte.xlsx");
    }

    [HttpGet("generated-questions")]
    public async Task<IResult> GetGeneratedQuestions(
        [FromQuery] Guid generatedAssigmentId)
    {
        return await HandleFile(
            new GetExcelGeneratedQuestionsQuery(generatedAssigmentId),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"questions-{generatedAssigmentId}.xlsx");
    }
}
