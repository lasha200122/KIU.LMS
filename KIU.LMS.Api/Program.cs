using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
{
    var logger = Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.File( 
            path: "logs/app-.log",                 
            rollingInterval: RollingInterval.Day,  
            retainedFileCountLimit: 7,             
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
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
    await app.ApplyMigrationsAsync(Log.Logger);
    
    app.UseExceptionHandler();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseRouting();
    app.UseCors("Default");

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(@"C:\inetpub\files"),
        RequestPath = "/files"
    });

    app.UseMiddleware<ActiveSessionMiddleware>();

    app.UseAuthorization();

    app.MapHub<NotificationHub>("/hubs/notification");
    app.MapHub<GeminiHub>("/hubs/gemini");

    app.MapControllers();

    app.Run();
}