using StockTech.Application.DTOs.Invoices;

namespace StockTech.Application.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto);
}
