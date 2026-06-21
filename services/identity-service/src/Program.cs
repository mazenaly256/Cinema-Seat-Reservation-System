using identity_service;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
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
                otlp.Endpoint = new Uri(builder.Configuration["OpenTelemetryExporter:DestinationEndpoint"]!);
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

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
