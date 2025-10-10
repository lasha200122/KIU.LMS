using KIU.LMS.Application.Features.Analytics.Queries;

namespace KIU.LMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController(ISender _sender) : ApiController(_sender)
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IResult> GetStudentAnalytics(Guid id) 
    {
        return await Handle<StudentAnalyticsQuery, StudentAnalyticsDto>(new StudentAnalyticsQuery(id));
    }

    [HttpGet("summary/{id}")]
    public async Task<IResult> GetStudentSummary(Guid id)
    {
        return await Handle<StudentSummaryQuery, object>(new StudentSummaryQuery(id));
    }

    [HttpGet("student-by-course")]
    [ProducesResponseType(typeof(StudentCourseAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IResult> GetStudentByCourse(
        [FromQuery] Guid studentId,
        [FromQuery] Guid courseId)
    {
        return await Handle<StudentCourseAnalyticsQuery, StudentCourseAnalyticsDto>
            (new StudentCourseAnalyticsQuery(studentId,  courseId));
    }
}
