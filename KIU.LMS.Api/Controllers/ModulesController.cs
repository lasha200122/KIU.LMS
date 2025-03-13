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

    [HttpGet("subs")]
    public async Task<IResult> GetSubModulesQuery([FromQuery] GetSubModulesQuery request) 
    {
        return await Handle<GetSubModulesQuery, PagedEntities<GetSubModulesQueryResponse>>(request);
    }

    [HttpGet("subs/{id}")]
    public async Task<IResult> GetSubModuleDetails(Guid id) 
    {
        return await Handle<GetSubModuleDetailsQuery, GetSubModuleDetailsQueryResponse>(new GetSubModuleDetailsQuery(id));
    }

    [HttpPost("subs")]
    public async Task<IResult> CreateSubModule([FromBody] CreateSubModuleCommand request) 
    {
        return await Handle(request);
    }

    [HttpPut("subs")]
    public async Task<IResult> UpdateSubModule([FromBody] UpdateSubModuleCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("subs/{id}")]
    public async Task<IResult> DeleteSubModule(Guid id) 
    {
        return await Handle(new DeleteSubModuleCommand(id));
    }
}
