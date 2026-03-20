namespace StockTech.Application.DTOs.Products;

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    string? Size,
    string? Color,
    string SKU,
    decimal Price,
    int Stock,
    bool IsActive
);

public record CreateProductVariantDto(
    string? Size,
    string? Color,
    string SKU,
    decimal Price,
    int Stock
);

public record CreateProductDto(
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int Stock,
    int MinStock,
    string? Category,
    List<CreateProductVariantDto>? Variants = null
);

public record UpdateProductDto(
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int MinStock,
    string? Category,
    bool IsActive,
    List<CreateProductVariantDto>? Variants = null
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
    DateTime CreatedAt,
    List<ProductVariantDto> Variants
);
