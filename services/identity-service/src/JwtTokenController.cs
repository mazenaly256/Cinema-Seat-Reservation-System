using Microsoft.AspNetCore.Mvc;

namespace identity_service;

[ApiController]
[Route("api/auth")]
public class JwtTokenController(AuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<string> GetAccessTokenAsync(IssueJwtTokenRequest request)
    {
        return (await authenticationService.IssueJwtTokenAsync(request.Email, request.Password));
    }
}
