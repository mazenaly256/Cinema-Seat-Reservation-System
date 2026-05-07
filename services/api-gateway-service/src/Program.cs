using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api-limiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(5);
        opt.PermitLimit = 200;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseRateLimiter();

app.MapReverseProxy()
    .RequireRateLimiting("api-limiter");

app.Run();
