namespace StockTech.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    // Manager
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    // Navigation
    public ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
