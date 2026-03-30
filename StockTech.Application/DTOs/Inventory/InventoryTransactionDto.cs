namespace StockTech.Application.DTOs.Inventory;

public record InventoryTransactionDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid? VariantId,
    string? VariantName,
    int Type,
    string TypeName,
    decimal Quantity,
    decimal PreviousStock,
    decimal NewStock,
    string? ReferenceNumber,
    DateTime TransactionDate,
    Guid? InvoiceId,
    Guid? PurchaseId,
    Guid? BranchId,
    string? BranchName,
    Guid? UserId = null,
    string? UserName = null
);
