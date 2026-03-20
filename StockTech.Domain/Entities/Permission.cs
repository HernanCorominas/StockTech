namespace StockTech.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "product:create", "invoice:delete"
    public string? Description { get; set; }
    
    // Many-to-many relationship with Role
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
