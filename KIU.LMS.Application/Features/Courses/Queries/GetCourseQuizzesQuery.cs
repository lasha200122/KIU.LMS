namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseQuizzesQuery(Guid CourseId, QuizType Type, bool IsTraining) : IRequest<Result<IEnumerable<GetCourseQuizzesQueryResponse>>>;

public sealed record GetCourseQuizzesQueryResponse(
    Guid Id,
    string Name,
    string Score,
    string TotalScore,
    string StartDate,
    string EndDate,
    string Course,
    int Order);


public sealed class GetCourseQuizzesQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetCourseQuizzesQuery, Result<IEnumerable<GetCourseQuizzesQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetCourseQuizzesQueryResponse>>> Handle(GetCourseQuizzesQuery request, CancellationToken cancellationToken)
    { 
        var result = await _unitOfWork.QuizRepository.GetMappedAsync(
            x => x.CourseId == request.CourseId && x.Type == request.Type && x.StartDateTime <= DateTimeOffset.UtcNow && x.IsTraining == request.IsTraining,
            x =>
                new GetCourseQuizzesQueryResponse(
                    x.Id,
                    x.Title,
                    x.ExamResults
                        .Where(r => r.StudentId == _current.UserId)
                        .OrderByDescending(r => r.CreateDate)
                        .Select(r =>
                            (decimal)r.CorrectAnswers / r.TotalQuestions * (x.Score ?? 0)
                        )
                        .Select(s => s.ToString())
                        .FirstOrDefault(),
                    x.Score.HasValue ? x.Score.Value.ToString() : "-",
                    x.StartDateTime.ToLocalTime().ToString("MMM dd, HH:mm"),
                    x.EndDateTime.HasValue ? x.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
                    x.Topic.Name,
                    x.Order),
            cancellationToken);

        return Result<IEnumerable<GetCourseQuizzesQueryResponse>>.Success(result);
    }
}