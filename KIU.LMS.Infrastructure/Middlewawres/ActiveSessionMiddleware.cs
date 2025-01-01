namespace KIU.LMS.Infrastructure.Middlewawres;

public class ActiveSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public ActiveSessionMiddleware(
        RequestDelegate next,
        IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        var sessionService = scope.ServiceProvider.GetRequiredService<IActiveSessionService>();

        if (currentUserService.IsAuthenticated)
        {
            var deviceId = currentUserService.DeviceId;
            if (string.IsNullOrEmpty(deviceId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "მოწყობილობის იდენტიფიკატორი არ არის მითითებული" });
                return;
            }

            var isActiveSession = await sessionService.IsActiveSessionAsync(currentUserService.UserId, deviceId);
            if (!isActiveSession)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "სესია ვადაგასულია" });
                return;
            }
        }

        await _next(context);
    }
}