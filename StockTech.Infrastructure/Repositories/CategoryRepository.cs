using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly StockTechDbContext _ctx;
    public CategoryRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _ctx.Categories.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _ctx.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetByNameAsync(string name) =>
        await _ctx.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

    public async Task AddAsync(Category category) => await _ctx.Categories.AddAsync(category);

    public void Update(Category category) => _ctx.Categories.Update(category);

    public void Delete(Category category)
    {
        category.IsActive = false;
        _ctx.Categories.Update(category);
    }
}
