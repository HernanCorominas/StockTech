using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Product : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; } = 0;
    public Guid? CategoryId { get; set; }
    public string? Image { get; set; }
    public virtual Category? Category { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public uint xmin { get; set; } // PostgreSQL internal transaction ID for optimistic concurrency (xid)

    // Navigation
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    
    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
