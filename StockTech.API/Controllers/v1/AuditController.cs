using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.Interfaces;
using Asp.Versioning;

namespace StockTech.API.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuditController : ControllerBase
{
    private readonly IAuditLogService _auditService;

    public AuditController(IAuditLogService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? entityName = null, 
        [FromQuery] string? action = null, 
        [FromQuery] DateTime? start = null, 
        [FromQuery] DateTime? end = null)
    {
        var logs = await _auditService.GetLogsByFiltersAsync(page, pageSize, entityName, action, start, end);
        return Ok(logs);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 50)
    {
        var logs = await _auditService.GetRecentLogsAsync(count);
        return Ok(logs);
    }
}
