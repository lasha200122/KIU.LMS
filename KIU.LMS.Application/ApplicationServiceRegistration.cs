namespace KIU.LMS.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Serilog.ILogger logger)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(thisAssembly);
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(thisAssembly));
        services.AddHttpContextAccessor();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        logger.Information("Layer loaded: {Layer} ", thisAssembly.GetName().Name);

        return services;
    }
}
