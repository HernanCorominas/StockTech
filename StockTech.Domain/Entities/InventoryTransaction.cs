using StockTech.Domain.Enums;

using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class InventoryTransaction : BaseEntity, ITenantEntity
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public Guid? VariantId { get; set; }
    public virtual ProductVariant? Variant { get; set; }
    
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    
    public TransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    
    public string? ReferenceNumber { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    public Guid? InvoiceId { get; set; }
    public Guid? PurchaseId { get; set; }

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
