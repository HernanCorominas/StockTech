using StockTech.Domain.Enums;

namespace StockTech.Application.DTOs.Inventory;

public record InventoryMovementItemDto(
    Guid? ProductId,
    Guid? VariantId,
    string? ProductName,
    Guid? CategoryId,
    string? SKU,
    decimal Quantity,
    decimal UnitCost,
    decimal? TaxRate
);

public record StockEntryRequest(
    Guid? SupplierId,
    string? SupplierName,
    Guid BranchId,
    List<InventoryMovementItemDto> Items,
    string? Notes,
    bool IsPurchase = true
);

public record StockExitRequest(
    Guid BranchId,
    List<InventoryMovementItemDto> Items,
    string? Notes,
    TransactionType Type = TransactionType.Sale
);
