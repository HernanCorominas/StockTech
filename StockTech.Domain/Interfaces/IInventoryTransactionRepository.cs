using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<InventoryTransaction>> GetAllAsync();
    Task AddAsync(InventoryTransaction transaction);
}
