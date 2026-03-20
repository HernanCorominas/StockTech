using StockTech.Domain.Enums;

namespace StockTech.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; } = 0.18m; // ITBIS 18%
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Paid;
    public string? Notes { get; set; }

    // Navigation
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
