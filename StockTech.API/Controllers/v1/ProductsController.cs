using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.DTOs.Products;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IExportService _exportService;

    public ProductsController(IProductService service, IExportService exportService)
    {
        _service = service;
        _exportService = exportService;
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var products = await _service.GetAllAsync();
        var bytes = await _exportService.ExportProductsToExcelAsync(products);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Productos_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var products = await _service.GetAllAsync();
        var bytes = await _exportService.ExportProductsToPdfAsync(products);
        return File(bytes, "application/pdf", $"Productos_{DateTime.Now:yyyyMMdd}.pdf");
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
    [Authorize(Policy = "RequireProductWrite")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireProductWrite")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireProductDelete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
