using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    // RBAC & Multi-Tenancy
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}
