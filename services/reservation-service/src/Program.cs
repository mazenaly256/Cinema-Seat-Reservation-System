using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using reservation_service.Data;
using reservation_service.Services.Implementations;
using reservation_service.Services.Interfaces;

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

        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapControllers();

app.MapOpenApi("/reservation-service/api-documentation");

app.Run();
