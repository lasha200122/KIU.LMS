namespace KIU.LMS.Application.Features.LeaderBoard;

public sealed record GetLeaderBoardCoursesQuery() : IRequest<Result<List<GetLeaderBoardCoursesQueryResponse>>>;

public sealed record GetLeaderBoardCoursesQueryResponse(Guid Id, string Name, List<GetLeaderBoardCoursesQueryQuizItem> Quizzes);

public sealed record GetLeaderBoardCoursesQueryQuizItem(Guid Id, string Name);

public sealed class GetLeaderBoardCoursesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetLeaderBoardCoursesQuery, Result<List<GetLeaderBoardCoursesQueryResponse>>>
{
    public async Task<Result<List<GetLeaderBoardCoursesQueryResponse>>> Handle(GetLeaderBoardCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.CourseRepository.GetAllAsync(cancellationToken);

        var response = new List<GetLeaderBoardCoursesQueryResponse>();

        foreach (var course in courses)
        {
            var quizzes = await _unitOfWork.QuizRepository.GetWhereAsync(x => x.CourseId == course.Id && x.PublicTill.HasValue && DateTimeOffset.UtcNow < x.PublicTill.Value, cancellationToken);

            if (quizzes is null || quizzes.Count() == 0)
                continue;

            var quizItems = quizzes.Select(x => new GetLeaderBoardCoursesQueryQuizItem(x.Id, x.Title)).ToList();

            response.Add(new GetLeaderBoardCoursesQueryResponse(course.Id, course.Name, quizItems));
        }

        return Result<List<GetLeaderBoardCoursesQueryResponse>>.Success(response);
    }
}