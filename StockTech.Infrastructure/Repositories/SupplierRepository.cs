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
        await _ctx.Suppliers.OrderBy(s => s.Name).ToListAsync();

    public async Task<Supplier?> GetByIdAsync(Guid id) =>
        await _ctx.Suppliers.FindAsync(id);

    public async Task AddAsync(Supplier supplier) => await _ctx.Suppliers.AddAsync(supplier);

    public void Update(Supplier supplier) => _ctx.Suppliers.Update(supplier);

    public void Delete(Supplier supplier) => _ctx.Suppliers.Remove(supplier);
}
