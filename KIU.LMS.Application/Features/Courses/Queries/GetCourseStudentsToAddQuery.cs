namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseStudentsToAddQuery(Guid Id) : IRequest<Result<ICollection<GetCourseStudentsToAddQueryResponse>>>;

public sealed record GetCourseStudentsToAddQueryResponse(Guid Id, string FullName, string Email);


public class GetCourseStudentsToAddQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseStudentsToAddQuery, Result<ICollection<GetCourseStudentsToAddQueryResponse>>>
{
    public async Task<Result<ICollection<GetCourseStudentsToAddQueryResponse>>> Handle(GetCourseStudentsToAddQuery request, CancellationToken cancellationToken)
    {
        var courseStudents = await _unitOfWork.UserCourseRepository.GetMappedAsync(x => x.CourseId == request.Id, x=> x.UserId, cancellationToken);

        var students = await _unitOfWork.UserRepository.GetMappedAsync(x => !courseStudents.Contains(x.Id), x => new GetCourseStudentsToAddQueryResponse(
            x.Id,
            $"{x.FirstName} {x.LastName}",
            x.Email));

        return Result<ICollection<GetCourseStudentsToAddQueryResponse>>.Success(students);
    }
}