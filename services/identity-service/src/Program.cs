using identity_service;
using MongoDB.Bson.Serialization.Conventions;

var builder = WebApplication.CreateBuilder(args);

var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

builder.Services.AddControllers();
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

app.MapControllers();

app.Run();
