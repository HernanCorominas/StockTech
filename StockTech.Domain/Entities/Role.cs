namespace StockTech.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "Administrator", "Seller"
    public string? Description { get; set; }
    
    // Many-to-many relationship with Permission
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
