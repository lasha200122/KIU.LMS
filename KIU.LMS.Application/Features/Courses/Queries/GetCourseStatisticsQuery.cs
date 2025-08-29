
namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseStatisticsQuery(Guid Id) : IRequest<Result<GetCourseStatisticsQueryResponse>>;

public sealed record GetCourseStatisticsQueryResponse(
    AssignmentStatistic C2RS,
    AssignmentStatistic IPEQ,
    AssignmentStatistic Projects,
    AssignmentStatistic MCQ);

public sealed record AssignmentStatistic(
    int Modules,
    int Banks,
    int Question,
    int Assignments);

//public class GetCourseStatisticsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseStatisticsQuery, Result<GetCourseStatisticsQueryResponse>>
//{
//    public async Task<Result<GetCourseStatisticsQueryResponse>> Handle(GetCourseStatisticsQuery request, CancellationToken cancellationToken)
//    {
//    }
//}
