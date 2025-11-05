namespace KIU.LMS.Api.Controllers;

[Route("api/courses")]
[Authorize]
public class CourseController(ISender sender) : ApiController(sender)
{
    #region Courses

    [HttpGet("tasks")]
    public async Task<IResult> GetAllTasks([FromQuery] GetAllAssignmentsQuery request) 
    {
        return await Handle<GetAllAssignmentsQuery, GetAllAssignmentsQueryResponse>(request);
    }

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

    [HttpPost("students/add/excel")]
    public async Task<IResult> CourseAddStudents([FromForm] CourseStudentsAddCommand request) 
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

    [HttpPost("topic/update")]
    public async Task<IResult> UpdateCourseTopic([FromBody] UpdateCourseTopicCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("topic/{id}")]
    public async Task<IResult> DeleteCourseTopic(Guid id) 
    {
        return await Handle(new DeleteCourseTopicCommand(id));
    }

    [HttpGet("topic/{id}")]
    public async Task<IResult> GetTopicDetails(Guid id) 
    {
        return await Handle<GetTopicDetailsQuery, GetTopicDetailsQueryResponse>(new GetTopicDetailsQuery(id));
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

    [HttpPost("assignment/solution/grade")]
    public async Task<IResult> GradeAssignmentSolution([FromBody] GradeAssignmentSolutionCommand request)
    {
        return await Handle(request);
    }

    [HttpPost("assignment")]
    public async Task<IResult> AddAssignment([FromBody] AddAssignmentCommand request) 
    {
        return await Handle(request);
    }

    [HttpPost("assignment-by-bank")]
    public async Task<IResult> AddAssigmentByBank([FromBody] AddAssigmentByModulesCommand request)
    {
        return await Handle(request);
    }

    [HttpPost("assignment/update")]
    public async Task<IResult> UpdateAssignment([FromBody] UpdateAssignmentCommand request) 
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

    [HttpGet("assignment/details/{id}")]
    public async Task<IResult> GetAssignmentDetails(Guid id) 
    {
        return await Handle<GetAssignmentDetailsQuery, GetAssignmentDetailsQueryResponse>(new GetAssignmentDetailsQuery(id));
    }

    [HttpDelete("assignments/{id}")]
    public async Task<IResult> DeleteAssignment(Guid id) 
    {
        return await Handle(new DeleteAssigmnentCommand(id));
    }

    [HttpPost("assignment/submit")]
    public async Task<IResult> SubmitAssignment([FromBody] SubmitAssignmentCommand request) 
    {
        return await Handle<SubmitAssignmentCommand, Guid>(request);
    }

    [HttpPost("assignment/ipeq/execute")]
    public async Task<IResult> IpeqExecute([FromBody] IpeqExecuteCommand request) 
    {
        return await Handle<IpeqExecuteCommand, string?>(request);
    }

    [HttpPost("assignment/solution/{id}")]
    public async Task<IResult> GradeSolution(Guid id) 
    {
        return await Handle(new GradeSolutionCommand(id));
    }

    [HttpGet("generated-assigments")]
    [ProducesResponseType(typeof(PagedEntities<GetGeneratedAssignmentsResult>), StatusCodes.Status200OK)]
    public async Task<IResult> GetGeneratedAssignments([FromQuery] GetGeneratedAssignmentsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }
    
    #endregion Assignments

    #region Quizzes
    [HttpGet("quizzes")]
    public async Task<IResult> GetQuizzes([FromQuery] GetCourseQuizzesQuery request) 
    {
        return await Handle<GetCourseQuizzesQuery, IEnumerable<GetCourseQuizzesQueryResponse>>(request);
    }


    [HttpGet("quizzzes/grid")]
    public async Task<IResult> GetQuizzesGrid([FromQuery] GetQuizzesQuery request) 
    {
        return await Handle<GetQuizzesQuery, PagedEntities<GetQuizzesQueryResponse>>(request);
    }

    [HttpGet("quiz/{id}")]
    public async Task<IResult> GetQuizDetails(Guid id) 
    {
        return await Handle<GetQuizDetailsQuery, GetQuizDetailsQueryResponse>(new GetQuizDetailsQuery(id));
    }

    [HttpDelete("quiz/{id}")]
    public async Task<IResult> DeleteQuiz(Guid id) 
    {
        return await Handle(new DeleteQuizCommand(id));
    }

    [HttpPut("quiz/{id}/start")]
    public async Task<IResult> StartQuiz(Guid id) 
    {
        return await Handle(new StartQuizCommand(id));
    }

    [HttpPut("quiz/reschedule")]
    public async Task<IResult> RescheduleQuiz([FromBody] RescheduleQuizCommand request) 
    {
        return await Handle(request);
    }

    #endregion Quizzes

    #region Solutions

    [HttpGet("solutions")]
    public async Task<IResult> AssignmentSolutions([FromQuery] StudentsSolutionListQuery request) 
    {
        return await Handle<StudentsSolutionListQuery, PagedEntities<StudentsSolutionListQueryResponse>>(request);
    }

    #endregion Solutions
}
