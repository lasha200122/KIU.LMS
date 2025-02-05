namespace KIU.LMS.Api.Controllers;

[Route("api/courses")]
[Authorize]
public class CourseController(ISender sender) : ApiController(sender)
{
    #region Courses

    [HttpGet("user")]
    public async Task<IResult> GetUserCourses()
    {
        return await Handle<GetUserCoursesQuery, IEnumerable<GetUserCoursesResponse>>(new GetUserCoursesQuery());
    }

    [HttpGet]
    public async Task<IResult> GetCourses([FromQuery] GetCoursesQuery request)
    {
        return await Handle<GetCoursesQuery, PagedEntities<GetCoursesQueryResponse>>(request);
    }

    [HttpPost]
    public async Task<IResult> CreateCourse([FromBody] CreateCourseCommand request)
    {
        return await Handle(request);
    }

    [HttpPut]
    public async Task<IResult> UpdateCourse([FromBody] UpdateCourseCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteCourse(Guid id)
    {
        return await Handle(new DeleteCourseCommand(id));
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetCourseDetails(Guid id)
    {
        return await Handle<GetCourseDetailsQuery, GetCourseDetailsQueryResponse>(new GetCourseDetailsQuery(id));
    }

    #endregion Courses

    #region Student Courses

    [HttpGet("students")]
    public async Task<IResult> GetCourses([FromQuery] GetCourseStudentsQuery request)
    {
        return await Handle<GetCourseStudentsQuery, PagedEntities<GetCourseStudentsQueryResponse>>(request);
    }

    [HttpGet("{id}/students/add")]
    public async Task<IResult> GetCourseStudentsToAdd(Guid id)
    {
        return await Handle<GetCourseStudentsToAddQuery, ICollection<GetCourseStudentsToAddQueryResponse>>(new GetCourseStudentsToAddQuery(id));
    }

    [HttpPost("students/add")]
    public async Task<IResult> CourseAddStudent([FromBody] CourseAddStudentCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("{id}/students")]
    public async Task<IResult> CourseDeleteStudent(Guid id)
    {
        return await Handle(new CourseDeleteStudentCommand(id));
    }

    #endregion Student Courses

    #region Course Meetings 

    [HttpPost("meetings")]
    public async Task<IResult> AddCourseMeeting([FromBody] AddCourseMeetingCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("meetings/{id}")]
    public async Task<IResult> DeleteCourseMeeting(Guid id)
    {
        return await Handle(new DeleteCourseMeetingCommand(id));
    }

    [HttpGet("meetings")]
    public async Task<IResult> GetCourseMeetings([FromQuery] GetCourseMeetingsQuery request)
    {
        return await Handle<GetCourseMeetingsQuery, ICollection<GetCourseMeetingsQueryResponse>>(request);
    }

    [HttpPut("meetings")]
    public async Task<IResult> UpdateCourseMeeting([FromBody] UpdateCourseMeetingCommand request)
    {
        return await Handle(request);
    }

    #endregion Course Meetings

    #region Teaching Plan

    [HttpGet("plan")]
    public async Task<IResult> GetTeachingPlan([FromQuery] GetTeachingPlanQuery request) 
    {
        return await Handle<GetTeachingPlanQuery, IEnumerable<GetTeachingPlanQueryResponse>>(request); 
    }

    #endregion Teaching Plan


    #region Topics

    [HttpGet("topics")]
    public async Task<IResult> GetTopics([FromQuery] GetTopicsQuery request) 
    {
        return await Handle<GetTopicsQuery, PagedEntities<GetTopicsQueryResponse>>(request);
    }

    [HttpPost("topic")]
    public async Task<IResult> AddCourseTopic([FromBody] AddCourseTopicCommand request)
    {
        return await Handle(request);
    }

    [HttpGet("topic-list")]
    public async Task<IResult> GetTopicsList([FromQuery] GetTopicsListQuery request) 
    {
        return await Handle<GetTopicsListQuery, IEnumerable<GetTopicsListQueryResponse>>(request);
    }

    #endregion Topics

    #region Assignments

    [HttpGet("assignment")]
    public async Task<IResult> GetCourseAssignments([FromQuery] GetCourseAssignmentsQuery request)
    {
        return await Handle<GetCourseAssignmentsQuery, IEnumerable<GetCourseAssignmentsQueryResponse>>(request);
    }

    [HttpPost("assignment")]
    public async Task<IResult> AddAssignment([FromBody] AddAssignmentCommand request) 
    {
        return await Handle(request);
    }

    [HttpGet("assignments")]
    public async Task<IResult> GetAssignments([FromQuery] GetAssignmentsQuery request) 
    {
        return await Handle<GetAssignmentsQuery, PagedEntities<GetAssignmentsQueryesponse>>(request);
    }


    [HttpGet("assignment/details")]
    public async Task<IResult> GetAssignmentDetails([FromQuery] GetAssignmentByIdQuery request) 
    {
        return await Handle<GetAssignmentByIdQuery, GetAssignmentByIdQueryResponse>(request);
    }

    [HttpPost("assignment/submit")]
    public async Task<IResult> SubmitAssignment([FromBody] SubmitAssignmentCommand request) 
    {
        return await Handle(request);
    }

    [HttpPost("assignment/ipeq/execute")]
    public async Task<IResult> IpeqExecute([FromBody] IpeqExecuteCommand request) 
    {
        return await Handle<IpeqExecuteCommand, string?>(request);
    }

    #endregion Assignments
}
