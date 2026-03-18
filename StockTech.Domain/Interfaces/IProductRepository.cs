using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}
