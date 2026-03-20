using StockTech.Domain.Enums;

namespace StockTech.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public TransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    
    public string? ReferenceNumber { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    public Guid? InvoiceId { get; set; }
    public Guid? PurchaseId { get; set; }
}
