using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.Interfaces;

namespace StockTech.API.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetMetrics() =>
        Ok(await _service.GetMetricsAsync());
}
