using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.DTOs.Invoices;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { message = "La factura debe tener al menos un item." });

        if (dto.Items.Any(i => i.Quantity <= 0))
            return BadRequest(new { message = "La cantidad de cada item debe ser mayor a cero." });

        try
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
