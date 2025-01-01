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

    builder.Services
    .AddApplicationServices(logger)
    .AddPersistenceServices(builder.Configuration, logger)
    .AddInfrastructureServices(builder.Configuration, logger);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    logger.Information("Starting web host");

    builder.Services.AddProblemDetails();

    var origins = builder.Configuration.GetSection("Origins").Value!.Split(';');

    builder.Services.AddCors(x => x.AddPolicy("Default", o =>
    {
        o.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(origins);
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

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
