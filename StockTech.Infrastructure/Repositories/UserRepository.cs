using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    protected readonly StockTechDbContext _ctx;
    public UserRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _ctx.Users.FindAsync(id);

    public async Task AddAsync(User user) => await _ctx.Users.AddAsync(user);
}
