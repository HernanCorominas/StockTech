using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.DTOs.SystemSettings;
using StockTech.Application.Interfaces;
using System.Security.Claims;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingService _service;
    private readonly IAuthService _authService;
    private readonly IAuditLogService _auditLogService;

    public SystemSettingsController(
        ISystemSettingService service,
        IAuthService authService,
        IAuditLogService auditLogService)
    {
        _service = service;
        _authService = authService;
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var result = await _service.GetByKeyAsync(key);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{key}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Upsert(string key, [FromBody] UpdateSystemSettingDto dto)
    {
        var currentSetting = await _service.GetByKeyAsync(key);
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name) ?? "System";
        var branchIdStr = User.FindFirstValue("BranchId");
        Guid? branchId = string.IsNullOrEmpty(branchIdStr) ? null : Guid.Parse(branchIdStr);

        // Sensitive check: IsTaxEnabled toggle off
        if (key == "IsTaxEnabled" && dto.Value.ToLower() == "false" && currentSetting?.Value.ToLower() == "true")
        {
            if (string.IsNullOrEmpty(dto.Password) || !Guid.TryParse(userIdStr, out var userId))
            {
                return BadRequest(new { message = "Password required to disable taxes." });
            }

            if (!await _authService.VerifyPasswordAsync(userId, dto.Password))
            {
                return Unauthorized(new { message = "Incorrect password." });
            }
        }

        var result = await _service.UpsertAsync(key, dto);

        return Ok(result);
    }
}
