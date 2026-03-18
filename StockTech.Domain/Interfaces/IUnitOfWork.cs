namespace StockTech.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClientRepository Clients { get; }
    IProductRepository Products { get; }
    IInvoiceRepository Invoices { get; }
    IPurchaseRepository Purchases { get; }
    IUserRepository Users { get; }
    Task<int> CommitAsync();
}
