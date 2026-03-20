namespace StockTech.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    // Navigation
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
