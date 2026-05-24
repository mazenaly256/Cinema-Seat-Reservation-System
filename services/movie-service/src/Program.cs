using FluentValidation;
using Microsoft.EntityFrameworkCore;
using movie_service.Data;
using movie_service.RequestDTOs;
using movie_service.Services.Implementations;
using movie_service.Services.Interfaces;
using movie_service.Validators;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(dbConnectionString))
{
    throw new InvalidOperationException("Connection string is not found.");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnectionString));
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();

builder.Services.AddScoped<IValidator<CreateMovieRequestDto>, CreateMovieRequestDtoValidator>();
builder.Services.AddScoped<IValidator<CreateShowtimeRequestDto>, CreateShowtimeRequestDtoValidator>();

builder.Services.AddOpenApi("movie-service-api-doc");

var app = builder.Build();

app.MapControllers();

app.MapOpenApi();

app.Run();
