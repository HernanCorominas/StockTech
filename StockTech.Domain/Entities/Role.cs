using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Role : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty; // e.g. "Administrator", "Seller"
    public string? Description { get; set; }
    
    public Guid? BranchId { get; set; }
    public virtual Branch? Branch { get; set; }
    
    // Many-to-many relationship with Permission
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    // Relationship with UserBranch
    public ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}
