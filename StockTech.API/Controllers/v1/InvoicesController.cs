using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.DTOs.Invoices;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly IExportService _exportService;

    public InvoicesController(IInvoiceService service, IExportService exportService)
    {
        _service = service;
        _exportService = exportService;
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var invoices = await _service.GetAllAsync();
        var bytes = await _exportService.ExportInvoicesToExcelAsync(invoices);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Facturas_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var invoices = await _service.GetAllAsync();
        var bytes = await _exportService.ExportInvoicesToPdfAsync(invoices);
        return File(bytes, "application/pdf", $"Facturas_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null) =>
        Ok(await _service.GetPagedAsync(page, pageSize, search));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
