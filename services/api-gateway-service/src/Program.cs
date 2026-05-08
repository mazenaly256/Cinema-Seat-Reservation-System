using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api-limiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(5);
        opt.PermitLimit = 1000;
        opt.QueueLimit = 0;
    });
});

builder.Services.AddRequestTimeouts(options =>
{
    options.AddPolicy("gateway-timeout", TimeSpan.FromSeconds(5));
});

var app = builder.Build();

app.UseRequestTimeouts();
app.UseRateLimiter();

app.MapReverseProxy()
    .WithRequestTimeout("gateway-timeout")
    .RequireRateLimiting("api-limiter");

app.Run();
