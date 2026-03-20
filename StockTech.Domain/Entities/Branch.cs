namespace StockTech.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;

    // Navigation
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
