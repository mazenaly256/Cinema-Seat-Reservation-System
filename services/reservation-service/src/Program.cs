using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ISeatService, SeatService>();

var app = builder.Build();

app.MapControllers();

app.Run();
