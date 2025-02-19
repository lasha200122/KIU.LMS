namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record StudentsSolutionListQuery(Guid Id, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<StudentsSolutionListQueryResponse>>>;

public sealed record StudentsSolutionListQueryResponse(
    Guid Id,
    Guid UserId,
    string Fullname,
    string Grade,
    string Feedback,
    GradingStatus Status);

public sealed class StudentsSolutionListQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<StudentsSolutionListQuery, Result<PagedEntities<StudentsSolutionListQueryResponse>>>
{
    public async Task<Result<PagedEntities<StudentsSolutionListQueryResponse>>> Handle(StudentsSolutionListQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.SolutionRepository.GetPaginatedWhereMappedAsync(
            x => x.AssignmentId == request.Id,
            request.PageNumber,
            request.PageSize,
            x => new StudentsSolutionListQueryResponse(
                x.Id,
                x.UserId,
                $"{x.User.FirstName} {x.User.LastName}",
                x.Grade,
                x.FeedBack,
                x.GradingStatus),
            x => x.CreateDate,
            cancellationToken);

        return Result<PagedEntities<StudentsSolutionListQueryResponse>>.Success(result);
    }
}