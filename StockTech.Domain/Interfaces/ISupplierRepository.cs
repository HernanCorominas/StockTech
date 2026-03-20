using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(Guid id);
    Task AddAsync(Supplier supplier);
    void Update(Supplier supplier);
    void Delete(Supplier supplier);
}
