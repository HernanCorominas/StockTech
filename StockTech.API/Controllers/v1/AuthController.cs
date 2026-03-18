using Microsoft.AspNetCore.Mvc;
using StockTech.Application.DTOs.Auth;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Login with username and password</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Usuario y contraseña son requeridos." });

        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

        return Ok(result);
    }
}
