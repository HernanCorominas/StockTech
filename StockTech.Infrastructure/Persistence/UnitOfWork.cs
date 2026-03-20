using Microsoft.EntityFrameworkCore;
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
    public ISupplierRepository Suppliers { get; }
    public IBranchRepository Branches { get; }
    public IUserRepository Users { get; }
    public IInventoryTransactionRepository InventoryTransactions { get; }

    public UnitOfWork(StockTechDbContext ctx)
    {
        _ctx = ctx;
        Clients = new ClientRepository(ctx);
        Products = new ProductRepository(ctx);
        Invoices = new InvoiceRepository(ctx);
        Purchases = new PurchaseRepository(ctx);
        Suppliers = new SupplierRepository(ctx);
        Branches = new BranchRepository(ctx);
        Users = new UserRepository(ctx);
        InventoryTransactions = new InventoryTransactionRepository(ctx);
    }

    public async Task<IEnumerable<T>> GetSetAsync<T>() where T : class => await _ctx.Set<T>().ToListAsync();

    public async Task<int> CommitAsync() => await _ctx.SaveChangesAsync();

    public void Dispose() => _ctx.Dispose();
}
