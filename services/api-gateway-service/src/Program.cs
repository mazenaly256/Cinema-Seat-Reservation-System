using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "CSRS",
            ValidAudience = "CSRS",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRequestTimeouts();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (httpContext, next) =>
{
    httpContext.Request.Headers["X-User-Id"] = httpContext.User.FindFirst("user_id")?.Value;
    httpContext.Request.Headers["X-User-Role"] = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

    await next();
});

app.MapReverseProxy()
    .WithRequestTimeout("gateway-timeout")
    .RequireRateLimiting("api-limiter")
    .RequireAuthorization();

app.Run();
