namespace StockTech.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClientRepository Clients { get; }
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IInvoiceRepository Invoices { get; }
    IPurchaseRepository Purchases { get; }
    ISupplierRepository Suppliers { get; }
    IBranchRepository Branches { get; }
    IUserRepository Users { get; }
    IInventoryTransactionRepository InventoryTransactions { get; }
    IAuditLogRepository AuditLogs { get; }
    IActivityLogRepository ActivityLogs { get; }
    INotificationRepository Notifications { get; }
    Task<IEnumerable<T>> GetSetAsync<T>() where T : class;
    System.Linq.IQueryable<T> GetQueryable<T>() where T : class;
    void Add<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void Remove<T>(T entity) where T : class;
    Task AddAsync<T>(T entity) where T : class;
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
}
