using KIU.LMS.Persistence.Database.Services;

namespace KIU.LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly DataMigrationService _migration;

    public MigrationController(DataMigrationService migration)
    {
        _migration = migration;
    }

    [HttpPost("sync-all")]
    public async Task<IActionResult> SyncAll()
    {
        await _migration.MigrateAllAsync();
        return Ok("✅ Full migration completed successfully!");
    }
}
