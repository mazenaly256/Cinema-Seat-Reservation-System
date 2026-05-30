using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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

var app = builder.Build();

app.MapControllers();

app.MapOpenApi("/movie-service/api-documentation");

app.UseCors();

app.Run();
