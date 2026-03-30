using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.DTOs.Inventory;
using StockTech.Application.Interfaces;
using StockTech.Domain.Enums;
using Asp.Versioning;

namespace StockTech.API.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryMovementService _inventoryService;

    public InventoryController(IInventoryMovementService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("kardex/{productId}")]
    public async Task<IActionResult> GetKardex(Guid productId)
    {
        var transactions = await _inventoryService.GetKardexAsync(productId);
        return Ok(transactions);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] Guid? branchId, 
        [FromQuery] Guid? productId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate, 
        [FromQuery] TransactionType? type,
        [FromQuery] Guid? userId)
    {
        var transactions = await _inventoryService.GetFilteredTransactionsAsync(branchId, productId, startDate, endDate, type, userId);
        return Ok(transactions);
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust([FromBody] ManualStockAdjustmentDto dto)
    {
        try
        {
            await _inventoryService.AdjustAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

