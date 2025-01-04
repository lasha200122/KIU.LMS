using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
{
    var logger = Log.Logger = new LoggerConfiguration()
        .WriteTo
            .Graylog(new GraylogSinkOptions
            {
                HostnameOrAddress = builder.Configuration.GetSection("GRAY_LOG_HOSTNAME_OR_ADDRESS").Value!,
                Port = int.Parse(builder.Configuration.GetSection("GRAY_LOG_PORT").Value!),
                Facility = "KIU.LMS",
                TransportType = TransportType.Udp
            })
        .WriteTo
            .Console()
        .CreateLogger();

    builder.Host.UseSerilog(logger);

    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromMinutes(1);
    });

    builder.Services
    .AddPersistenceServices(builder.Configuration, logger)
    .AddInfrastructureServices(builder.Configuration, logger)
    .AddApplicationServices(logger);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "KIU LMS API",
            Version = "v1",
            Description = "API Documentation for KIU LMS"
        });

        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    });

    logger.Information("Starting web host");

    builder.Services.AddProblemDetails();

    var origins = builder.Configuration.GetSection("Origins").Value!.Split(';');

    builder.Services.AddCors(x => x.AddPolicy("Default", o =>
    {
        o.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(origins)
            .AllowCredentials();
    }));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
}

var app = builder.Build();
{
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("Default");

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseMiddleware<ActiveSessionMiddleware>();

    app.UseAuthorization();

    app.MapHub<NotificationHub>("/hubs/notification");

    app.MapControllers();

    app.Run();
}
