using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Purchase : BaseEntity, ITenantEntity
{
    public string PurchaseNumber { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
