using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}
