using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Services;
using UserApi.Services.Interfaces;

namespace UserApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [AllowAnonymous]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        try
        {
            var response = authService.Login(request.Username, request.Password);
            return response is null ? Unauthorized() : Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Problem("Authentication is not configured.", statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
