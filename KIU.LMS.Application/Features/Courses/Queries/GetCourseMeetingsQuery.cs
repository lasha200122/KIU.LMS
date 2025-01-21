namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseMeetingsQuery(Guid Id, DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<Result<ICollection<GetCourseMeetingsQueryResponse>>>;

public sealed record GetCourseMeetingsQueryResponse(Guid Id, string Name, string Url, DateTimeOffset StartTime, DateTimeOffset EndTime);

public class GetCourseMeetingsQueryValidator : AbstractValidator<GetCourseMeetingsQuery> 
{
    public GetCourseMeetingsQueryValidator() 
    {
        RuleFor(x => x.StartDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .NotNull();

        RuleFor(x => x.Id)
            .NotNull();
    }
}

public class GetCourseMeetingsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseMeetingsQuery, Result<ICollection<GetCourseMeetingsQueryResponse>>>
{
    public async Task<Result<ICollection<GetCourseMeetingsQueryResponse>>> Handle(GetCourseMeetingsQuery request, CancellationToken cancellationToken)
    {
        var meetings = await _unitOfWork.CourseMeetingRepository.GetMappedAsync(
            x => x.StartTime >= request.StartDate && x.StartTime <= request.EndDate && x.CourseId == request.Id,
            x => new GetCourseMeetingsQueryResponse(
                x.Id,
                x.Name,
                x.Url,
                x.StartTime,
                x.EndTime),
            cancellationToken
            );

        return Result<ICollection<GetCourseMeetingsQueryResponse>>.Success(meetings);
    }
}