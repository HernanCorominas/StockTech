using StockTech.Domain.Entities;

namespace StockTech.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "VIEW_PRODUCTS"
    public string Description { get; set; } = string.Empty;

    // Navigation
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
