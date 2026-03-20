using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId);
    Task AddAsync(InventoryTransaction transaction);
}
