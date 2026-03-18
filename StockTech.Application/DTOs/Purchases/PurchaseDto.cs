namespace StockTech.Application.DTOs.Purchases;

public record CreatePurchaseItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitCost
);

public record CreatePurchaseDto(
    string Supplier,
    List<CreatePurchaseItemDto> Items,
    string? Notes
);

public record PurchaseItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitCost,
    decimal LineTotal
);

public record PurchaseDto(
    Guid Id,
    string PurchaseNumber,
    string Supplier,
    DateTime PurchaseDate,
    decimal Total,
    string? Notes,
    List<PurchaseItemDto> Items,
    DateTime CreatedAt
);
