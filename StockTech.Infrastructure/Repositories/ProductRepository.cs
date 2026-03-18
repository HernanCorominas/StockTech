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
        await _ctx.Products.FindAsync(id);

    public async Task AddAsync(Product product) => await _ctx.Products.AddAsync(product);

    public void Update(Product product) => _ctx.Products.Update(product);

    public void Delete(Product product)
    {
        product.IsActive = false;
        _ctx.Products.Update(product);
    }
}
