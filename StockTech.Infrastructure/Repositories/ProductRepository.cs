using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    protected readonly StockTechDbContext _ctx;
    public ProductRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _ctx.Products.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _ctx.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
    {
        var query = _ctx.Products
            .Include(p => p.Variants)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || (p.SKU != null && p.SKU.Contains(search)));
        }
        
        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(p => p.Name)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
                               
        return (items, totalCount);
    }

    public async Task AddAsync(Product product) => await _ctx.Products.AddAsync(product);

    public void Update(Product product) => _ctx.Products.Update(product);

    public void Delete(Product product)
    {
        product.IsActive = false;
        _ctx.Products.Update(product);
    }
}
