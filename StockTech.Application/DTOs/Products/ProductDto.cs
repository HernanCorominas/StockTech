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
    int MinStock,
    Guid? CategoryId,
    int? InitialStock = 0,
    Guid? BranchId = null,
    Guid? SupplierId = null,
    string? Image = null,
    List<CreateProductVariantDto>? Variants = null
);

public record UpdateProductVariantDto(
    Guid? Id,
    string? Size,
    string? Color,
    string SKU,
    decimal Price,
    int Stock,
    bool IsActive
);

public record UpdateProductDto(
    string Name,
    string? Description,
    string? SKU,
    decimal Price,
    decimal Cost,
    int MinStock,
    Guid? CategoryId,
    bool IsActive,
    Guid? BranchId = null,
    Guid? SupplierId = null,
    string? Image = null,
    List<UpdateProductVariantDto>? Variants = null
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
    Guid? CategoryId,
    string? CategoryName,
    bool IsActive,
    bool LowStock,
    Guid BranchId,
    Guid? SupplierId,
    string? SupplierName,
    string? Image,
    DateTime CreatedAt,
    List<ProductVariantDto> Variants
);
