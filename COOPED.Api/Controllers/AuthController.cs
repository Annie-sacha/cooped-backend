using COOPED.Application.DTOs.Auth;
using COOPED.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace COOPED.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var resultat = await _authService.LoginAsync(request);
        return resultat is null
            ? Unauthorized(new { message = "Email ou mot de passe incorrect." })
            : Ok(resultat);
    }
}