using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Supplier : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    // Navigation
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
