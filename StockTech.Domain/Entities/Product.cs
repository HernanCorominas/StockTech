namespace StockTech.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Stock { get; set; }
    public int MinStock { get; set; } = 0;
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;

    public uint xmin { get; set; } // PostgreSQL internal transaction ID for optimistic concurrency (xid)

    // Navigation
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}
