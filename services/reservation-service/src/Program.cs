using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using reservation_service.Data;
using reservation_service.Services.Implementations;
using reservation_service.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

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
