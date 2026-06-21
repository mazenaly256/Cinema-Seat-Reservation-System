using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;       // this is for tracing-logging correlation
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("API Gateway"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(otlp =>
            {
                otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                otlp.Endpoint = new Uri(builder.Configuration["OpenTelemetryExporter:TracingDestinationEndpoint"]!);
                otlp.Headers = $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{builder.Configuration["OpenTelemetryExporter:CloudInstanceID"]}:{builder.Configuration["OpenTelemetryExporter:CloudInstanceApiToken"]}"))}";
            });
    })
    .WithLogging(logging =>
    {
        logging.AddOtlpExporter(otlp =>
        {
            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
            otlp.Endpoint = new Uri(builder.Configuration["OpenTelemetryExporter:LoggingDestinationEndpoint"]!);
            otlp.Headers = $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{builder.Configuration["OpenTelemetryExporter:CloudInstanceID"]}:{builder.Configuration["OpenTelemetryExporter:CloudInstanceApiToken"]}"))}";
        });
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(otlp =>
            {
                otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
                otlp.Endpoint = new Uri(builder.Configuration["OpenTelemetryExporter:MetricsDestinationEndpoint"]!);
                otlp.Headers = $"Authorization=Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{builder.Configuration["OpenTelemetryExporter:CloudInstanceID"]}:{builder.Configuration["OpenTelemetryExporter:CloudInstanceApiToken"]}"))}";
            });
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api-limiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(5);
        opt.PermitLimit = 1000;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddRequestTimeouts(options =>
{
    options.AddPolicy("gateway-timeout", TimeSpan.FromSeconds(8));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "CSRS",
            ValidAudience = "CSRS",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors();

app.UseRequestTimeouts();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (httpContext, next) =>
{
    httpContext.Request.Headers["X-User-Id"] = httpContext.User.FindFirst("user_id")?.Value;
    httpContext.Request.Headers["X-User-Role"] = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

    await next();
});

app.MapReverseProxy()
    .WithRequestTimeout("gateway-timeout")
    .RequireRateLimiting("api-limiter")
    .RequireAuthorization();

app.Run();
