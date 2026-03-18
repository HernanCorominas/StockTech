namespace StockTech.Application.DTOs.Products;

public record CreateProductDto(
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int Stock,
    int MinStock,
    string? Category
);

public record UpdateProductDto(
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int MinStock,
    string? Category,
    bool IsActive
);

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int Stock,
    int MinStock,
    string? Category,
    bool IsActive,
    bool LowStock,
    DateTime CreatedAt
);
