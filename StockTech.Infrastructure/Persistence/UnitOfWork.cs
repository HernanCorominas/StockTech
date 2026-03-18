using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;
using StockTech.Infrastructure.Repositories;

namespace StockTech.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockTechDbContext _ctx;

    public IClientRepository Clients { get; }
    public IProductRepository Products { get; }
    public IInvoiceRepository Invoices { get; }
    public IPurchaseRepository Purchases { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(StockTechDbContext ctx)
    {
        _ctx = ctx;
        Clients = new ClientRepository(ctx);
        Products = new ProductRepository(ctx);
        Invoices = new InvoiceRepository(ctx);
        Purchases = new PurchaseRepository(ctx);
        Users = new UserRepository(ctx);
    }

    public async Task<int> CommitAsync() => await _ctx.SaveChangesAsync();

    public void Dispose() => _ctx.Dispose();
}
