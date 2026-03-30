using StockTech.Domain.Enums;

using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Client : BaseEntity, ITenantEntity
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

    public Guid? BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
}
