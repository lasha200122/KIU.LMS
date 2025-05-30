namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModuleDetailsQuery(Guid Id) : IRequest<Result<GetModuleDetailsQueryResponse>>;

public sealed record GetModuleDetailsQueryResponse(
    Guid Id,
    string Name,
    string Homeworks,
    string Classworks,
    string MCQs,
    string Projects,
    string C2RS,
    string IPEQ);

public sealed class GetModuleDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetModuleDetailsQuery, Result<GetModuleDetailsQueryResponse>>
{
    public async Task<Result<GetModuleDetailsQueryResponse>> Handle(GetModuleDetailsQuery request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.ModuleRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.ModuleBanks);

        if (module is null)
            return Result<GetModuleDetailsQueryResponse>.Failure("Can't find module");

        var response = new GetModuleDetailsQueryResponse(
            module.Id,
            module.Name,
            module.ModuleBanks.Where(x => x.Type == SubModuleType.Homework).Count().ToString(),
            module.ModuleBanks.Where(x => x.Type == SubModuleType.Classwork).Count().ToString(),
            module.ModuleBanks.Where(x => x.Type == SubModuleType.MCQ).Count().ToString(),
            module.ModuleBanks.Where(x => x.Type == SubModuleType.Project).Count().ToString(),
            module.ModuleBanks.Where(x => x.Type == SubModuleType.C2RS).Count().ToString(),
            module.ModuleBanks.Where(x => x.Type == SubModuleType.IPEQ).Count().ToString());

        return Result<GetModuleDetailsQueryResponse>.Success(response);
    }
}
