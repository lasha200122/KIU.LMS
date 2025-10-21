namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetAllAssignmentsQuery(Guid CourseId) : IRequest<Result<GetAllAssignmentsQueryResponse>>;

public sealed record TaskListItem(
    Guid Id,
    string Name,
    string Topic,
    string Score,
    string TotalScore,
    string Deadline,
    DateTimeOffset? DeadlineUtc,
    string Type,
    string RedirectUrl,
    bool IsTraining);

public sealed record GetAllAssignmentsQueryResponse(List<TaskListItem> Tasks);

public sealed class GetAllAssignmentsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService current) : IRequestHandler<GetAllAssignmentsQuery, Result<GetAllAssignmentsQueryResponse>>
{
    public async Task<Result<GetAllAssignmentsQueryResponse>> Handle(GetAllAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await unitOfWork.AssignmentRepository.GetMappedAsync(
            x => x.CourseId == request.CourseId && !x.IsDeleted && x.StartDateTime.HasValue && x.StartDateTime <= DateTimeOffset.UtcNow && x.EndDateTime.HasValue,
            x => new TaskListItem(
                x.Id,
                x.Name,
                x.Topic.Name,
                x.Solutions.Any(x => x.UserId == current.UserId) ? x.Solutions.Where(x => x.UserId == current.UserId).OrderByDescending(x => x.CreateDate).First().Grade : string.Empty,
                x.Score.HasValue ? x.Score.Value.ToString() : "-",
                x.EndDateTime.HasValue ? x.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
                x.EndDateTime,
                x.Type.ToString(),
                RedirectUrl(x.Type, x.IsTraining, x.Id),
                x.IsTraining),
            cancellationToken);

        var mcqs = await unitOfWork.QuizRepository.GetMappedAsync(
            x => x.CourseId == request.CourseId && !x.IsDeleted && x.StartDateTime <= DateTimeOffset.UtcNow && x.EndDateTime.HasValue,
            x => new TaskListItem(
                x.Id,
                x.Title,
                x.Topic.Name,
                x.ExamResults.Any(x => x.StudentId == current.UserId) ? x.ExamResults.Where(x => x.StudentId == current.UserId).OrderByDescending(x => x.CreateDate).First().Score.ToString() : string.Empty,
                x.Score.HasValue ? x.Score.Value.ToString() : "-",
                x.EndDateTime.HasValue ? x.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
                x.EndDateTime,
                x.Type.ToString(),
                RedirectUrl(x.IsTraining, x.Id),
                x.IsTraining),
            cancellationToken);

        var result = assignments
            .Union(mcqs)
            .OrderByDescending(x => x.DeadlineUtc)
            .ToList();

        return Result<GetAllAssignmentsQueryResponse>.Success(new GetAllAssignmentsQueryResponse(result));
    }

    private static string RedirectUrl(AssignmentType type, bool isTraining, Guid id) 
    {
        var baseUrl = isTraining ? "learning/training" : "learning/graiding";

        return type switch
        {
            AssignmentType.Homework => $"{baseUrl}/c2rs/{id}",
            AssignmentType.IPEQ => $"{baseUrl}/ipeq/{id}",
            AssignmentType.Project => $"{baseUrl}/project/{id}",
            _ => string.Empty
        };
    }

    private static string RedirectUrl(bool isTraining, Guid id)
    {
        var baseUrl = isTraining ? "learning/training" : "learning/graiding";

        return $"{baseUrl}/mcq/{id}";
    }
}