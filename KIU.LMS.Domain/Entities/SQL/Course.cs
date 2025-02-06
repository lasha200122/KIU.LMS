namespace KIU.LMS.Domain.Entities.SQL;

public class Course : Aggregate
{
    public string Name { get; private set; } = null!;

    private List<UserCourse> _userCourses = new();
    public IReadOnlyCollection<UserCourse> UserCourses => _userCourses;

    private List<CourseMaterial> _materials = new();
    public IReadOnlyCollection<CourseMaterial> Materials => _materials;

    private List<CourseMeeting> _meetings = new();
    public IReadOnlyCollection<CourseMeeting> Meetings => _meetings;

    private List<Exam> _exams = new();
    public IReadOnlyCollection<Exam> Exams => _exams;

    private List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments;
    private List<Topic> _topics = new();
    public IReadOnlyCollection<Topic> Topics => _topics;

    private List<Quiz> _quizes = new();
    public IReadOnlyCollection<Quiz> Quizzes => _quizes;
    public Course() { }

    public Course(
        Guid id,
        string name,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        Name = name;
        Validate(this);
    }

    public void Update(string name, Guid updateUserId)
    {
        Name = name;
        Update(updateUserId, DateTimeOffset.UtcNow);
        Validate(this);
    }

    private void Validate(Course course)
    {
        if (string.IsNullOrEmpty(course.Name))
            throw new Exception("კურსის სახელი სავალდებულოა");
    }
}