using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.Interfaces;
using StockTech.Application.DTOs.Auth;

namespace StockTech.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null) return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        return Ok(result);
    }

    [HttpPost("verify-password")]
    [Authorize]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        
        var isValid = await _authService.VerifyPasswordAsync(Guid.Parse(userIdStr), request.Password);
        return Ok(new { isValid });
    }

    [HttpPost("recovery/reset-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetAdminPassword([FromBody] ResetAdminRequest request)
    {
        var result = await _authService.ResetAdminPasswordAsync(request.RecoveryKey, request.NewPassword);
        if (!result) return BadRequest("Recovery key incorrecta o el usuario admin no existe.");
        return Ok(new { message = "Contraseña de administrador restablecida con éxito." });
    }
}

public record VerifyPasswordRequest(string Password);
public record ResetAdminRequest(string RecoveryKey, string NewPassword);
