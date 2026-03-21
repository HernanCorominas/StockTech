using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.DTOs.Reports;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string? branchId = null)
    {
        var result = await _service.GetSummaryAsync(new ReportFilterDto(from, to, branchId));
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string? branchId = null)
    {
        var bytes = await _service.ExportToExcelAsync(new ReportFilterDto(from, to, branchId));
        var fileName = $"StockTech_Reporte_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
