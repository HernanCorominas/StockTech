namespace StockTech.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    // RBAC
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
