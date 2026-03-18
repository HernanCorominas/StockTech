namespace StockTech.Domain.Entities;

public class Purchase : BaseEntity
{
    public string PurchaseNumber { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
