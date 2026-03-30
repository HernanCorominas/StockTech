using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    protected readonly StockTechDbContext _ctx;
    public PurchaseRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Purchase>> GetAllAsync() =>
        await _ctx.Purchases.AsNoTracking()
            .Include(p => p.Items).ThenInclude(item => item.Product)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync();

    public async Task<IEnumerable<Purchase>> GetByDateRangeAsync(DateTime from, DateTime to) =>
        await _ctx.Purchases.AsNoTracking()
            .Include(p => p.Items).ThenInclude(item => item.Product)
            .Where(p => p.PurchaseDate >= from && p.PurchaseDate <= to)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id) =>
        await _ctx.Purchases
            .Include(p => p.Items).ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Purchase purchase) => await _ctx.Purchases.AddAsync(purchase);
}
