namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetUserCoursesQuery() : IRequest<Result<IEnumerable<GetUserCoursesResponse>>>;

public sealed record GetUserCoursesResponse(Guid Id, string Value);

public class GetUserCoursesQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetUserCoursesQuery, Result<IEnumerable<GetUserCoursesResponse>>>
{
    public async Task<Result<IEnumerable<GetUserCoursesResponse>>> Handle(GetUserCoursesQuery request, CancellationToken cancellationToken)
    {
        if (_current.Role == "Admin")
        {
            var course = await _unitOfWork.CourseRepository.GetWhereIncludedAsync(x => true);

            var adminResponse = course.Select(c => new GetUserCoursesResponse(c.Id, c.Name));

            return Result<IEnumerable<GetUserCoursesResponse>>.Success(adminResponse);
        }
        var courses = await _unitOfWork.UserCourseRepository.GetWhereIncludedAsync(x => x.UserId == _current.UserId, x => x.Course);

        var response = courses.Select(c => new GetUserCoursesResponse(c.CourseId, c.Course.Name));

        return Result<IEnumerable<GetUserCoursesResponse>>.Success(response);
    }
}