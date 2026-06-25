using identity_service;
using identity_service.Custom_Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
    .ConfigureResource(resource => resource.AddService("Identity Service"))
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

var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

var identity_db_connectionString = builder.Configuration.GetSection("MongoDbSettings")["ConnectionString"];

if (string.IsNullOrWhiteSpace(identity_db_connectionString))
{
    throw new InvalidOperationException("Connection string is not found.");
}

builder.Services.AddSingleton<IMongoClient>(new MongoClient(identity_db_connectionString));

builder.Services.AddControllers();
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/problem+json";

        var status = ex switch
        {
            InvalidCredentialsException => 400,
            _ => 500
        };

        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = ex switch
            {
                InvalidCredentialsException => "Invalid credentials",
                _ => "Internal Server Error"
            },

            Detail = ex switch
            {
                InvalidCredentialsException => "Incorrect Email or Password, they are not registered.",
                _ => "An error occurred while processing your request."
            }
        });
    });
});

app.MapControllers();

app.Run();
