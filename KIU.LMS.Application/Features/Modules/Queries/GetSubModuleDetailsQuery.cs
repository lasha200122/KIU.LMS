namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetSubModuleDetailsQuery(Guid Id) : IRequest<Result<GetSubModuleDetailsQueryResponse>>;

public sealed record GetSubModuleDetailsQueryResponse(Guid Id, string Name, string? Problem, string? Code);

public sealed class GetSubModuleDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetSubModuleDetailsQuery, Result<GetSubModuleDetailsQueryResponse>>
{
    public async Task<Result<GetSubModuleDetailsQueryResponse>> Handle(GetSubModuleDetailsQuery request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.SubModuleRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (module is null)
            return Result<GetSubModuleDetailsQueryResponse>.Failure("Can't find sub module");

        var response = new GetSubModuleDetailsQueryResponse(
            module.Id,
            module.Name,
            module.Problem,
            module.Code);

        return Result<GetSubModuleDetailsQueryResponse>.Success(response);
    }
}
