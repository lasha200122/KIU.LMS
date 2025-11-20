using DataMigration.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace DataMigration;

public static class MigrationDependencies
{
    public static IServiceCollection AddMigrationDependencies(this IServiceCollection services)
    {
        //services.AddDbContext<KiuLmsContext>();
        return services.AddScoped<DataMigrationService>();
    }
}