using DataMigration.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace DataMigration;

public static class MigrationDependencies
{
    public static void AddMigrationDependencies(this IServiceCollection services)
    {
        //services.AddDbContext<KiuLmsContext>();
        services.AddScoped<DataMigrationService>();
    }
}