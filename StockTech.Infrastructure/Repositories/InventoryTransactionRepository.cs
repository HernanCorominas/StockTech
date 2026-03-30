using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly StockTechDbContext _ctx;
    public InventoryTransactionRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId) =>
        await _ctx.InventoryTransactions.AsNoTracking()
            .Where(t => t.ProductId == productId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task<IEnumerable<InventoryTransaction>> GetAllAsync() =>
        await _ctx.InventoryTransactions.AsNoTracking()
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

    public async Task AddAsync(InventoryTransaction transaction) =>
        await _ctx.InventoryTransactions.AddAsync(transaction);
}
