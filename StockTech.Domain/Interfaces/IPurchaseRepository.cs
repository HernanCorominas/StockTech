using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IPurchaseRepository
{
    Task<IEnumerable<Purchase>> GetAllAsync();
    Task<IEnumerable<Purchase>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Purchase?> GetByIdAsync(Guid id);
    Task AddAsync(Purchase purchase);
}
