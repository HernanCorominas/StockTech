using StockTech.Application.DTOs.Inventory;
using StockTech.Application.DTOs.Purchases;
using StockTech.Domain.Entities;
using StockTech.Domain.Enums;

namespace StockTech.Application.Interfaces;

public interface IInventoryMovementService
{
    Task<PurchaseDto?> ProcessEntryAsync(StockEntryRequest request);
    Task ProcessExitAsync(StockExitRequest request);
    Task AdjustAsync(ManualStockAdjustmentDto dto); // Manual stock adjustment (Entry/Adjustment/Transfer)

    // Kardex & Stock Logic
    Task LogTransactionAsync(Guid productId, decimal quantity, TransactionType type, string? referenceNumber = null, Guid? invoiceId = null, Guid? purchaseId = null, Guid? variantId = null);
    Task<IEnumerable<InventoryTransaction>> GetKardexAsync(Guid productId);

    Task<IEnumerable<InventoryTransactionDto>> GetFilteredTransactionsAsync(
        Guid? branchId, 
        Guid? productId, 
        DateTime? startDate, 
        DateTime? endDate, 
        TransactionType? type, 
        Guid? userId = null);
    
    // Queries
    Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync();
    Task<PurchaseDto?> GetPurchaseByIdAsync(Guid id);
    Task<IEnumerable<InventoryTransaction>> GetAllTransactionsAsync();
}
