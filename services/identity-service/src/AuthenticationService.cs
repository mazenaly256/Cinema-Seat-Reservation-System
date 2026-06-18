using identity_service.Custom_Exceptions;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace identity_service;

public class AuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMongoCollection<ApplicationUser> _usersCollection;

    public AuthenticationService(IConfiguration configuration, IMongoClient mongoClient, ILogger<AuthenticationService> logger)
    {
        _logger = logger;
        _configuration = configuration;
        _usersCollection = mongoClient.GetDatabase(configuration.GetSection("MongoDbSettings")["DatabaseName"]).GetCollection<ApplicationUser>("users");
    }

    public async Task<string> IssueJwtTokenAsync(string Email, string Password)
    {
        var user = await GetUserFromDatabaseAsync(Email, Password);

        if (user is null)
        {
            _logger.LogWarning("Failed loging attempt | Email: {Email}", Email);

            throw new InvalidCredentialsException();
        }

        string? secretKey = _configuration.GetSection("JwtSettings")["SecretKey"];

        if (secretKey is null)
        {
            _logger.LogCritical("Missing JWT Secret Key | Signing Credentials For JWT Does Not Exist");

            throw new InvalidOperationException();
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var claims = new List<Claim>
        {
            new("user_id", user.UserId),
            new("user_name", user.Name)
        };

        if (user.Role is not null)
            claims.Add(new(ClaimTypes.Role, user.Role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            Issuer = "CSRS",
            Audience = "CSRS"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string jwtTokenString = tokenHandler.WriteToken(securityToken);

        _logger.LogInformation("User with email: {Email} has been issued a JWT successfully.", Email);

        return jwtTokenString;
    }

    private async Task<ApplicationUser?> GetUserFromDatabaseAsync(string Email, string Password)
    {
        var user = await _usersCollection.Find(x => x.Email == Email).SingleOrDefaultAsync();
        bool passwordCheck = BCrypt.Net.BCrypt.Verify(Password, user?.PasswordHash);

        return passwordCheck ? user : null;
    }
}
