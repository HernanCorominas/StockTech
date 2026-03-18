using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Invoice?> GetByIdAsync(Guid id);
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
}
