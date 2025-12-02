namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetTeachingPlanQuery(Guid CourseId, bool? IsTraining) : IRequest<Result<IEnumerable<GetTeachingPlanQueryResponse>>>;

public sealed record GetTeachingPlanQueryResponse(
    Guid Id, 
    string Topic,
    string Date,
    string Time,
    IEnumerable<AssignmentItem> C2RS,
    IEnumerable<AssignmentItem> MCQs,
    IEnumerable<AssignmentItem> IPEQs,
    IEnumerable<AssignmentItem> Projects,
    IEnumerable<Guid> Files);

public sealed record AssignmentItem(Guid Id, string Number, string Deadline, bool IsTraining, string RedirectUrl, DateTimeOffset? StartTime);

public class GetTeachingPlanQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetTeachingPlanQuery, Result<IEnumerable<GetTeachingPlanQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetTeachingPlanQueryResponse>>> Handle(GetTeachingPlanQuery request, CancellationToken cancellationToken)
    {
        var fileIds = await _unitOfWork.CourseMaterialRepository.GetMappedAsync(
            x => x.CourseId == request.CourseId,
            x => x.Id,
            cancellationToken
        );

        var result = await _unitOfWork.TopicRepository.GetSortedMappedAsync(
            x => x.CourseId == request.CourseId,
            x => new GetTeachingPlanQueryResponse(
                x.Id,
                x.Name,
                x.StartDateTime.Value.ToLocalTime().ToString("dd/MM/yyyy")?? string.Empty,
                $"{x.StartDateTime.Value.ToLocalTime().ToString("HH:mm")?? string.Empty} - {x.EndDateTime.Value.ToLocalTime().ToString("HH:mm")??string.Empty}",
                x.Assignments.Where(a => a.Type == AssignmentType.C2RS && (!request.IsTraining.HasValue || request.IsTraining.Value == a.IsTraining))
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString(), GetDateTime(y.EndDateTime), y.IsTraining, RedirectUrl(y.Type, y.IsTraining, y.Id), y.StartDateTime)),
                x.Quizzes.Where(a => a.Type == QuizType.MCQ && (!request.IsTraining.HasValue || request.IsTraining.Value == a.IsTraining))
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString(), GetDateTime(y.EndDateTime), y.IsTraining, RedirectUrl(y.IsTraining, y.Id), y.StartDateTime)),
                x.Assignments.Where(a => a.Type == AssignmentType.IPEQ && (!request.IsTraining.HasValue || request.IsTraining.Value == a.IsTraining))
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString(), GetDateTime(y.EndDateTime), y.IsTraining, RedirectUrl(y.Type, y.IsTraining, y.Id), y.StartDateTime)),
                x.Assignments.Where(a => a.Type == AssignmentType.Project && (!request.IsTraining.HasValue || request.IsTraining.Value == a.IsTraining))
                    .OrderBy(a => a.Order)
                    .Select(y => new AssignmentItem(y.Id, y.Order.ToString(), GetDateTime(y.EndDateTime), y.IsTraining, RedirectUrl(y.Type, y.IsTraining, y.Id), y.StartDateTime)),
                fileIds
            ),
            x => x.StartDateTime,
            false,
            cancellationToken
        );

        return Result<IEnumerable<GetTeachingPlanQueryResponse>>.Success(result);
    }

    private static string GetDateTime(DateTimeOffset? dateTime)
    {
        return dateTime.HasValue ? dateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty;
    }

    private static string RedirectUrl(AssignmentType type, bool isTraining, Guid id)
    {
        var baseUrl = isTraining ? "/learning/training" : "/learning/graiding";

        return type switch
        {
            AssignmentType.Homework => $"{baseUrl}/c2rs/{id}",
            AssignmentType.IPEQ => $"{baseUrl}/ipeq/{id}",
            AssignmentType.Project => $"{baseUrl}/project/{id}",
            AssignmentType.C2RS => $"{baseUrl}/c2rs/{id}",
            _ => string.Empty
        };
    }

    private static string RedirectUrl(bool isTraining, Guid id)
    {
        var baseUrl = isTraining ? "/learning/training" : "/learning/graiding";

        return $"{baseUrl}/mcq/{id}";
    }
}