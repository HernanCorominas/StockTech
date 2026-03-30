namespace StockTech.Application.DTOs.Suppliers;

public record CreateSupplierDto(
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string TaxId,
    Guid? BranchId
);

public record SupplierDto(
    Guid Id,
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string TaxId,
    Guid? BranchId,
    string? BranchName,
    DateTime CreatedAt
);
