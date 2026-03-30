using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(Guid id);
    Task AddAsync(Supplier supplier);
    Task<Supplier?> GetByNameAsync(string name);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(Supplier supplier);
}
