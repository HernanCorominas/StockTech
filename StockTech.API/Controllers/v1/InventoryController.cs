using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.Services;
using Asp.Versioning;

namespace StockTech.API.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("kardex/{productId}")]
    public async Task<IActionResult> GetKardex(Guid productId)
    {
        var transactions = await _inventoryService.GetKardexAsync(productId);
        return Ok(transactions);
    }
}
