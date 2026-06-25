using identity_service.Custom_Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace identity_service;

[ApiController]
[Route("api/auth")]
public class JwtTokenController(AuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<string>> GetAccessTokenAsync(IssueJwtTokenRequest request)
    {
        string jwtToken = await authenticationService.IssueJwtTokenAsync(request.Email, request.Password);

        return jwtToken;
    }
}
