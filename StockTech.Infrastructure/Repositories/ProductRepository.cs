using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    protected readonly StockTechDbContext _ctx;
    public ProductRepository(StockTechDbContext ctx) => _ctx = ctx;

    public IQueryable<Product> AsQueryable() => _ctx.Products;

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _ctx.Products.AsNoTracking().Include(p => p.Supplier).Include(p => p.Category).Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _ctx.Products
            .Include(p => p.Variants)
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string[]? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null,
        bool? lowStock = null, string? stockStatus = null, 
        Guid? supplierId = null, Guid? branchId = null)
    {
        var query = _ctx.Products.AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (branchId.HasValue && branchId != Guid.Empty)
        {
            query = query.Where(p => p.BranchId == branchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || (p.SKU != null && p.SKU.ToLower().Contains(searchLower)));
        }

        if (categoryId != null && categoryId.Length > 0)
        {
            var validItemIds = categoryId
                .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            if (validItemIds.Any())
            {
                query = query.Where(p => p.CategoryId != null && validItemIds.Contains(p.CategoryId.Value));
            }
        }

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (supplierId.HasValue) query = query.Where(p => p.SupplierId == supplierId.Value);
        if (lowStock.HasValue && lowStock.Value)
        {
            query = query.Where(p => p.Stock <= 15);
        }

        if (!string.IsNullOrWhiteSpace(stockStatus))
        {
            query = stockStatus.ToLower() switch
            {
                "critico" => query.Where(p => p.Stock <= 5),
                "bajo" => query.Where(p => p.Stock > 5 && p.Stock <= 15),
                "normal" => query.Where(p => p.Stock > 15),
                _ => query
            };
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
