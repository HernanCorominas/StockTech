using StockTech.Application.DTOs.Common;
using StockTech.Application.DTOs.Invoices;

namespace StockTech.Application.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<PagedResult<InvoiceDto>> GetPagedAsync(int page, int pageSize, string? search);
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto);
    Task CancelAsync(Guid id); // Cancels invoice and reverses stock
    Task<string> GetInvoicePdfAsync(Guid id); // Retorna la URL del PDF generado
}
