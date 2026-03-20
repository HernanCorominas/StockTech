using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly StockTechDbContext _ctx;
    public BranchRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Branch>> GetAllAsync() =>
        await _ctx.Branches.OrderBy(b => b.Name).ToListAsync();

    public async Task<Branch?> GetByIdAsync(Guid id) =>
        await _ctx.Branches.FindAsync(id);

    public async Task AddAsync(Branch branch) => await _ctx.Branches.AddAsync(branch);

    public void Update(Branch branch) => _ctx.Branches.Update(branch);

    public void Delete(Branch branch) => _ctx.Branches.Remove(branch);
}
