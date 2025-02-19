namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetTeachingPlanQuery(Guid CourseId) : IRequest<Result<IEnumerable<GetTeachingPlanQueryResponse>>>;


public sealed record GetTeachingPlanQueryResponse(
    Guid Id, 
    string Topic,
    string Date,
    string Time,
    IEnumerable<AssignmentItem> Homeworks,
    IEnumerable<AssignmentItem> ClassWorks,
    IEnumerable<AssignmentItem> MCQs,
    IEnumerable<AssignmentItem> IPEQs,
    IEnumerable<AssignmentItem> Projects);

public sealed record AssignmentItem(Guid Id, string Number);


public class GetTeachingPlanQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetTeachingPlanQuery, Result<IEnumerable<GetTeachingPlanQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetTeachingPlanQueryResponse>>> Handle(GetTeachingPlanQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TopicRepository.GetSortedMappedAsync(
            x => x.CourseId == request.CourseId,
            x => new GetTeachingPlanQueryResponse(
                x.Id,
                x.Name,
                x.StartDateTime.ToLocalTime().ToString("dd/MM/yyyy"),
                $"{x.StartDateTime.ToLocalTime().ToString("HH:mm")} - {x.EndDateTime.ToLocalTime().ToString("HH:mm")}",
                x.Assignments.Where(a => a.Type == AssignmentType.Homework && a.StartDateTime.HasValue && a.StartDateTime <= DateTimeOffset.UtcNow)
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString())),
                x.Assignments.Where(a => a.Type == AssignmentType.ClassWork && a.StartDateTime.HasValue && a.StartDateTime <= DateTimeOffset.UtcNow)
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString())),
                x.Quizzes.Where(a => a.Type == QuizType.MCQ && a.StartDateTime <= DateTimeOffset.UtcNow)
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString())),
                x.Assignments.Where(a => a.Type == AssignmentType.IPEQ && a.StartDateTime.HasValue && a.StartDateTime <= DateTimeOffset.UtcNow)
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString())),
                x.Assignments.Where(a => a.Type == AssignmentType.Project && a.StartDateTime.HasValue && a.StartDateTime <= DateTimeOffset.UtcNow)
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString()))
            ),
            x => x.StartDateTime,
            false,
            cancellationToken
        );

        return Result<IEnumerable<GetTeachingPlanQueryResponse>>.Success(result);
    }
}