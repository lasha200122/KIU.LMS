using Anthropic.SDK;
using Microsoft.Extensions.Options;

namespace KIU.LMS.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();

        var jwtSetting = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        services.AddSingleton(jwtSetting!);

        var emailSetting = configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>()!;
        services.AddSingleton(emailSetting!);

        var frontSetting = configuration.GetSection(nameof(FrontSettings)).Get<FrontSettings>()!;
        services.AddSingleton(frontSetting!);

        var geminiSetting = configuration.GetSection(nameof(GeminiSettings)).Get<GeminiSettings>()!;
        services.AddSingleton(geminiSetting!);

        var claudeSetting = configuration.GetSection(nameof(ClaudeSettings)).Get<ClaudeSettings>()!;
        services.AddSingleton(claudeSetting!);

        services.AddAuthorization();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSetting.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.Configure<ClaudeSettings>(configuration.GetSection("ClaudeSettings"));

        services.AddSingleton<AnthropicClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ClaudeSettings>>().Value;
            var client = new AnthropicClient();
            client.Auth = new APIAuthentication(settings.ApiKey);
            //client.AnthropicVersion = settings.AnthropicVersion;
            return client;
        });

        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IExcelProcessor, ExcelProcessor>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IActiveSessionService, ActiveSessionService>();
        services.AddScoped<IEmailProcessingService, EmailProcessingService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IGeminiService, GeminiService>();
        services.AddScoped<IGradingService, GradingService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IClaudeService, ClaudeService>();
        services.AddScoped<IFileService, FileService>();

        logger.Information("Layer loaded: {Layer} ", thisAssembly.GetName().Name);

        return services;
    }
}
