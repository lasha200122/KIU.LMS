namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseDetailsQuery(Guid Id) : IRequest<Result<GetCourseDetailsQueryResponse>>;

public sealed record GetCourseDetailsQueryResponse(Guid Id, string Name, int Students, string StartDate);

public class GetCourseDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseDetailsQuery, Result<GetCourseDetailsQueryResponse>>
{
    public async Task<Result<GetCourseDetailsQueryResponse>> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.UserCourses);

        if (course is null)
            return Result<GetCourseDetailsQueryResponse>.Failure("Can't find course");

        var response = new GetCourseDetailsQueryResponse(course.Id, course.Name, course.UserCourses.Count(), course.CreateDate.ToString("MMMM/yyyy"));

        return Result<GetCourseDetailsQueryResponse>.Success(response);
    }
}