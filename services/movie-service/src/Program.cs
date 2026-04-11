using Microsoft.EntityFrameworkCore;
using movie_service.Data;
using movie_service.Services.Implementations;
using movie_service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException("Connection string is not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddControllers();

builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

app.MapControllers();

app.Run();
