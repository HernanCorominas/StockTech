using StockTech.Domain.Entities;
using StockTech.Domain.Enums;

namespace StockTech.Application.Services;

public interface IInventoryService
{
    Task LogTransactionAsync(Guid productId, decimal quantity, TransactionType type, string? referenceNumber = null, Guid? invoiceId = null, Guid? purchaseId = null);
    Task<IEnumerable<InventoryTransaction>> GetKardexAsync(Guid productId);
}
