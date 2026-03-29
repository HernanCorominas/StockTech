using System.Linq;
using StockTech.Domain.Entities;

using System.Linq;
using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IProductRepository
{
    IQueryable<Product> AsQueryable();
    Task<IEnumerable<Product>> GetAllAsync();
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string[]? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null,
        bool? lowStock = null, string? stockStatus = null,
        Guid? supplierId = null, Guid? branchId = null);

    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}
