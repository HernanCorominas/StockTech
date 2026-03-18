using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    protected readonly StockTechDbContext _ctx;
    public ClientRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Client>> GetAllAsync() =>
        await _ctx.Clients.Where(c => c.IsActive).OrderByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<Client?> GetByIdAsync(Guid id) =>
        await _ctx.Clients.FindAsync(id);

    public async Task<Client?> GetByDocumentAsync(string document) =>
        await _ctx.Clients.FirstOrDefaultAsync(c => c.Document == document);

    public async Task AddAsync(Client client) => await _ctx.Clients.AddAsync(client);

    public void Update(Client client) => _ctx.Clients.Update(client);

    public void Delete(Client client)
    {
        client.IsActive = false;
        _ctx.Clients.Update(client);
    }
}
