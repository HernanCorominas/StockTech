using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.DTOs.Reports;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _service.GetSummaryAsync(new ReportFilterDto(from, to));
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var bytes = await _service.ExportToExcelAsync(new ReportFilterDto(from, to));
        var fileName = $"StockTech_Reporte_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
