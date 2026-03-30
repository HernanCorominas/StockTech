using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTech.Application.Interfaces;
using Asp.Versioning;

namespace StockTech.API.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly IStorageService _storageService;

    public DocumentsController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadDocument([FromQuery] string fileUrl)
    {
        try
        {
            var bytes = await _storageService.DownloadFileAsync(fileUrl);
            var fileName = Path.GetFileName(fileUrl);
            
            return File(bytes, "application/pdf", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("El documento solicitado no existe.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al descargar el documento: {ex.Message}");
        }
    }
}
