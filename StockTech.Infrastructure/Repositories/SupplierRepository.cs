using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly StockTechDbContext _ctx;
    public SupplierRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Supplier>> GetAllAsync() =>
        await _ctx.Suppliers.AsNoTracking().Include(s => s.Branch).OrderBy(s => s.Name).ToListAsync();

    public async Task<Supplier?> GetByIdAsync(Guid id) =>
        await _ctx.Suppliers.FindAsync(id);

    public async Task AddAsync(Supplier supplier) => await _ctx.Suppliers.AddAsync(supplier);

    public async Task<Supplier?> GetByNameAsync(string name) =>
        await _ctx.Suppliers.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());

    public async Task UpdateAsync(Supplier supplier) { _ctx.Suppliers.Update(supplier); await Task.CompletedTask; }

    public async Task DeleteAsync(Supplier supplier) { _ctx.Suppliers.Remove(supplier); await Task.CompletedTask; }
}
