namespace KIU.LMS.Application.Features.Modules.Queries;

public sealed record GetModuleBankDetailsQuery(Guid Id) : IRequest<Result<GetModuleBankDetailsQueryResponse>>;

public sealed record GetModuleBankDetailsQueryResponse(Guid Id, string Name, SubModuleType Type);

public sealed class GetSubModuleDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetModuleBankDetailsQuery, Result<GetModuleBankDetailsQueryResponse>>
{
    public async Task<Result<GetModuleBankDetailsQueryResponse>> Handle(GetModuleBankDetailsQuery request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.ModuleBankRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (module is null)
            return Result<GetModuleBankDetailsQueryResponse>.Failure("Can't find sub module");

        var response = new GetModuleBankDetailsQueryResponse(
            module.Id,
            module.Name,
            module.Type);

        return Result<GetModuleBankDetailsQueryResponse>.Success(response);
    }
}
