using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class ProductVariant : BaseEntity, ITenantEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? Size { get; set; }
    public string? Color { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
