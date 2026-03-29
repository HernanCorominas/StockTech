using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "RequireAdminRole")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _service;

    public AuditLogsController(IAuditLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] DateTime? start = null, 
        [FromQuery] DateTime? end = null)
    {
        DateTime? startUtc = start.HasValue ? DateTime.SpecifyKind(start.Value, DateTimeKind.Utc) : null;
        DateTime? endUtc = end.HasValue ? DateTime.SpecifyKind(end.Value.AddDays(1).AddTicks(-1), DateTimeKind.Utc) : null;
        
        return Ok(await _service.GetLogsByFiltersAsync(page, pageSize, null, null, startUtc, endUtc));
    }
}
