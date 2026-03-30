using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;
using StockTech.Infrastructure.Repositories;

namespace StockTech.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly StockTechDbContext _ctx;
    private IDbContextTransaction? _transaction;

    public IClientRepository Clients { get; }
    public ICategoryRepository Categories { get; }
    public IProductRepository Products { get; }
    public IInvoiceRepository Invoices { get; }
    public IPurchaseRepository Purchases { get; }
    public ISupplierRepository Suppliers { get; }
    public IBranchRepository Branches { get; }
    public IUserRepository Users { get; }
    public IInventoryTransactionRepository InventoryTransactions { get; }
    public IAuditLogRepository AuditLogs { get; }
    public IActivityLogRepository ActivityLogs { get; }
    public INotificationRepository Notifications { get; }

    public UnitOfWork(StockTechDbContext ctx)
    {
        _ctx = ctx;
        Clients = new ClientRepository(ctx);
        Categories = new CategoryRepository(ctx);
        Products = new ProductRepository(ctx);
        Invoices = new InvoiceRepository(ctx);
        Purchases = new PurchaseRepository(ctx);
        Suppliers = new SupplierRepository(ctx);
        Branches = new BranchRepository(ctx);
        Users = new UserRepository(ctx);
        InventoryTransactions = new InventoryTransactionRepository(ctx);
        AuditLogs = new AuditLogRepository(ctx);
        ActivityLogs = new ActivityLogRepository(ctx);
        Notifications = new NotificationRepository(ctx);
    }

    public async Task<IEnumerable<T>> GetSetAsync<T>() where T : class => await _ctx.Set<T>().ToListAsync();
    public IQueryable<T> GetQueryable<T>() where T : class => _ctx.Set<T>();
    public void Add<T>(T entity) where T : class => _ctx.Set<T>().Add(entity);
    public void Update<T>(T entity) where T : class => _ctx.Set<T>().Update(entity);
    public void Remove<T>(T entity) where T : class => _ctx.Set<T>().Remove(entity);
    public async Task AddAsync<T>(T entity) where T : class => await _ctx.Set<T>().AddAsync(entity);

    public async Task<int> CommitAsync() => await _ctx.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _transaction = await _ctx.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        var strategy = _ctx.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        var strategy = _ctx.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _ctx.Dispose();
    }
}
