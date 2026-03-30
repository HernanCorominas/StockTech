using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;

namespace StockTech.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;

    public RolesController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "RequireUserRead")]
    public async Task<IActionResult> GetRoles() => Ok(await _userService.GetRolesAsync());

    [HttpGet("permissions")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetPermissions() => Ok(await _userService.GetPermissionsAsync());

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create([FromBody] RoleRequestDto request)
    {
        var role = new Role { Name = request.Name, Description = request.Description };
        return Ok(await _userService.CreateRoleAsync(role, request.PermissionIds));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RoleRequestDto request)
    {
        var role = new Role { Id = id, Name = request.Name, Description = request.Description };
        await _userService.UpdateRoleAsync(role, request.PermissionIds);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteRoleAsync(id);
        return NoContent();
    }
}

public record RoleRequestDto(string Name, string Description, List<Guid> PermissionIds);
