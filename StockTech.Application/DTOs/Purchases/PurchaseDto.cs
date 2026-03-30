namespace StockTech.Application.DTOs.Purchases;

public record CreatePurchaseItemDto(
    Guid? ProductId,
    Guid? VariantId,
    string? ProductName,
    Guid? CategoryId,
    string? SKU,
    int Quantity,
    decimal UnitCost,
    decimal? TaxRate = 0
);

public record CreatePurchaseDto(
    Guid SupplierId,
    Guid? BranchId,
    List<CreatePurchaseItemDto> Items,
    string? Notes
);

public record PurchaseItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitCost,
    decimal TaxRate,
    decimal TaxAmount,
    decimal LineTotal
);

public record PurchaseDto(
    Guid Id,
    string PurchaseNumber,
    Guid? SupplierId,
    string SupplierName,
    Guid? BranchId,
    string? BranchName,
    DateTime PurchaseDate,
    decimal SubTotal,
    decimal TaxTotal,
    decimal Total,
    string? Notes,
    List<PurchaseItemDto> Items,
    DateTime CreatedAt
);
