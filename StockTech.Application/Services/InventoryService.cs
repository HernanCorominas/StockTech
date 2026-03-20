using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Enums;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public InventoryService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task LogTransactionAsync(Guid productId, decimal quantity, TransactionType type, string? referenceNumber = null, Guid? invoiceId = null, Guid? purchaseId = null)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        
        if (product == null) return;

        decimal previousStock = product.Stock;
        decimal newStock = type == TransactionType.Purchase || (type == TransactionType.Adjustment && quantity > 0)
            ? previousStock + Math.Abs(quantity)
            : previousStock - Math.Abs(quantity);

        var transaction = new InventoryTransaction
        {
            ProductId = productId,
            Type = type,
            Quantity = Math.Abs(quantity),
            PreviousStock = previousStock,
            NewStock = newStock,
            ReferenceNumber = referenceNumber,
            InvoiceId = invoiceId,
            PurchaseId = purchaseId,
            TransactionDate = DateTime.UtcNow
        };

        await _unitOfWork.InventoryTransactions.AddAsync(transaction);
        
        // Low Stock Check
        if (newStock <= product.MinStock && previousStock > product.MinStock)
        {
            await _notificationService.SendLowStockAlertAsync(product.Name, (int)newStock);
        }
        
        // Note: The product stock itself is updated in the calling service (Invoice/Purchase)
        // to keep the domain logic intact and avoid side effects in the logger.
        // We just record what the stock WILL BE or IS after the operation.
    }

    public async Task<IEnumerable<InventoryTransaction>> GetKardexAsync(Guid productId)
    {
        return await _unitOfWork.InventoryTransactions.GetByProductIdAsync(productId);
    }
}
