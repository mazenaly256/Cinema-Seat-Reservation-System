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
        try
        {
            string jwtToken = await authenticationService.IssueJwtTokenAsync(request.Email, request.Password);

            return jwtToken;
        }
        catch(InvalidCredentialsException ex)
        {
            return BadRequest($"{ex.Message} User with entered email and password does not exist in DB.");
        }
    }
}
