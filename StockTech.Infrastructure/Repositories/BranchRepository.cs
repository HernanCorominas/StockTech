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
        await _ctx.Branches.Include(b => b.Manager).AsNoTracking().OrderBy(b => b.Name).ToListAsync();

    public async Task<Branch?> GetByIdAsync(Guid id) =>
        await _ctx.Branches.Include(b => b.Manager).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Branch?> GetByNameAsync(string name) =>
        await _ctx.Branches.FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());

    public async Task AddAsync(Branch branch) => await _ctx.Branches.AddAsync(branch);

    public void Update(Branch branch) => _ctx.Branches.Update(branch);

    public void Delete(Branch branch) => _ctx.Branches.Remove(branch);
}
