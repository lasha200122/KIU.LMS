using KIU.LMS.Application.Features.Modules.Commands;
using KIU.LMS.Application.Features.Modules.Queries;

namespace KIU.LMS.Api.Controllers;

[Route("api/modules")]
[Authorize]
public class ModulesController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IResult> CreateModule([FromBody] CreateModuleCommand request)
    {
        return await Handle(request);
    }

    [HttpPut]
    public async Task<IResult> UpdateModule([FromBody] UpdateModuleCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteModule(Guid id) 
    {
        return await Handle(new DeleteModuleCommand(id));
    }

    [HttpGet]
    public async Task<IResult> GetModules([FromQuery] GetModulesQuery request) 
    {
        return await Handle<GetModulesQuery, PagedEntities<GetModulesQueryResponse>>(request);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetModuleDetails(Guid id) 
    {
        return await Handle<GetModuleDetailsQuery, GetModuleDetailsQueryResponse>(new GetModuleDetailsQuery(id));
    }

    [HttpGet("banks")]
    public async Task<IResult> GetBankModulesQuery([FromQuery] GetModulesBankQuery request) 
    {
        return await Handle<GetModulesBankQuery, PagedEntities<GetModulesBankQueryResponse>>(request);
    }

    [HttpGet("banks/{id}")]
    public async Task<IResult> GetBankModuleDetails(Guid id) 
    {
        return await Handle<GetModuleBankDetailsQuery, GetModuleBankDetailsQueryResponse>(new GetModuleBankDetailsQuery(id));
    }

    [HttpPost("banks")]
    public async Task<IResult> CreateBankModule([FromBody] CreateBankModuleCommand request) 
    {
        return await Handle(request);
    }

    [HttpPut("banks")]
    public async Task<IResult> UpdateBankModule([FromBody] UpdateModuleBankCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("banks/{id}")]
    public async Task<IResult> DeleteBankModule(Guid id) 
    {
        return await Handle(new DeleteModuleBankCommand(id));
    }
    
    [HttpPost("banks/{id}/submodules")]
    public async Task<IResult> CreateSubModule(Guid id, [FromBody] CreateSubModuleCommand request)
    {
        request = request with { ModuleBankId = id };
        return await Handle(request);
    }

    [HttpPut("banks/submodules")]
    public async Task<IResult> UpdateSubModule([FromBody] UpdateSubModuleCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("banks/submodules/{id}")]
    public async Task<IResult> DeleteSubModule(Guid id)
    {
        return await Handle(new DeleteSubModuleCommand(id));
    }

    [HttpGet("banks/{id}/submodules")]
    public async Task<IResult> GetSubModules(Guid id, [FromQuery] GetSubModulesQuery request)
    {
        request = request with { ModuleBankId = id };
        return await Handle<GetSubModulesQuery, PagedEntities<GetSubModulesQueryResponse>>(request);
    }

    [HttpGet("banks/submodules/{id}")]
    public async Task<IResult> GetSubModuleDetails(Guid id)
    {
        return await Handle<GetSubModuleByIdQuery, SubModuleDto>(new GetSubModuleByIdQuery(id));
    }

    [HttpGet("modules/{id}/submodules/list")]
    public async Task<IResult> GetSubModuleList(Guid id, [FromQuery] GetSubModulesByModuleQuery request)
    {
        request = request with { ModuleId = id };
        return await Handle<GetSubModulesByModuleQuery, List<SubModuleListDto>>(request);
    }

    [HttpPost("modules/import/submodules")]
    public async Task<IResult> ImportSubModules([FromForm] IFormFile file, [FromQuery] Guid moduleId)
    {
        return await Handle(new ImportC2RSCommand(file, moduleId));
    }
}
