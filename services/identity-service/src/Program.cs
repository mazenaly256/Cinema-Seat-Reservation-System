using identity_service;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

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

app.MapControllers();

app.Run();
