using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using reservation_service.Data;
using reservation_service.Services.Implementations;
using reservation_service.Services.Interfaces;
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
    .ConfigureResource(resource => resource.AddService("Reservation Service"))
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

var dbConnectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException("Connection string is not found.");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnectionString));
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ISeatService, SeatService>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers =
        [
            new OpenApiServer
            {
                Url = builder.Configuration["API_Gateway_URL"],
                Description = "API Gateway"
            }
        ];

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            }
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            }
        );

        return Task.CompletedTask;
    });
});

builder.Services.AddHttpClient("movie-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["movieServiceBaseUrl"]!);
})
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(2, retryAttemptNumber => TimeSpan.FromMilliseconds(200 * retryAttemptNumber)))
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(20)));

var app = builder.Build();

app.MapControllers();

app.MapOpenApi("/reservation-service/api-documentation");

app.Run();
