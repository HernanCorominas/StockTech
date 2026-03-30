using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using StockTech.Application.DTOs.Purchases;
using StockTech.Application.DTOs.Inventory;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IInventoryMovementService _service;

    public PurchasesController(IInventoryMovementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllPurchasesAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetPurchaseByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        try
        {
            // Map CreatePurchaseDto to StockEntryRequest
            var entryRequest = new StockEntryRequest(
                dto.SupplierId,
                null,
                dto.BranchId ?? Guid.Empty,
                dto.Items.Select(i => new InventoryMovementItemDto(
                    i.ProductId,
                    i.VariantId,
                    i.ProductName,
                    i.CategoryId,
                    i.SKU,
                    i.Quantity,
                    i.UnitCost,
                    i.TaxRate
                )).ToList(),
                dto.Notes,
                IsPurchase: true
            );

            var result = await _service.ProcessEntryAsync(entryRequest);
            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
