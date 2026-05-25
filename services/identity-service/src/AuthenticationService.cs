using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace identity_service;

public class AuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<ApplicationUser> _usersCollection;

    public AuthenticationService(IConfiguration configuration, IMongoClient mongoClient)
    {
        _configuration = configuration;
        _usersCollection = mongoClient.GetDatabase(configuration.GetSection("MongoDbSettings")["DatabaseName"]).GetCollection<ApplicationUser>("users");
    }

    public async Task<string?> IssueJwtTokenAsync(string Email, string Password)
    {
        var user = await GetUserFromDatabaseAsync(Email, Password);

        if (user is null)
            return null;


        string secretKey = _configuration.GetSection("JwtSettings")["SecretKey"]!;
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

        return jwtTokenString;
    }

    private async Task<ApplicationUser?> GetUserFromDatabaseAsync(string Email, string Password)
    {
        var user = await _usersCollection.Find(x => x.Email == Email).SingleOrDefaultAsync();
        bool passwordCheck = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);

        return passwordCheck ? user : null;
    }
}
