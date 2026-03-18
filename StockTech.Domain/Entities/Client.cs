using StockTech.Domain.Enums;

namespace StockTech.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty; // Cédula or RNC
    public ClientType ClientType { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
