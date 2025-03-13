namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModuleDetailsQuery(Guid Id) : IRequest<Result<GetModuleDetailsQueryResponse>>;

public sealed record GetModuleDetailsQueryResponse(
    Guid Id,
    string Name,
    string Homeworks,
    string Classworks,
    string MCQs,
    string Projects);

public sealed class GetModuleDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetModuleDetailsQuery, Result<GetModuleDetailsQueryResponse>>
{
    public async Task<Result<GetModuleDetailsQueryResponse>> Handle(GetModuleDetailsQuery request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.ModuleRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.SubModules);

        if (module is null)
            return Result<GetModuleDetailsQueryResponse>.Failure("Can't find module");

        var response = new GetModuleDetailsQueryResponse(
            module.Id,
            module.Name,
            module.SubModules.Where(x => x.SubModuleType == SubModuleType.Homework).Count().ToString(),
            module.SubModules.Where(x => x.SubModuleType == SubModuleType.Classwork).Count().ToString(),
            module.SubModules.Where(x => x.SubModuleType == SubModuleType.MCQ).Count().ToString(),
            module.SubModules.Where(x => x.SubModuleType == SubModuleType.Project).Count().ToString());

        return Result<GetModuleDetailsQueryResponse>.Success(response);
    }
}
