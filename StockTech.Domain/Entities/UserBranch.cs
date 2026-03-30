using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTech.Domain.Entities;

public class UserBranch
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // This allows different roles per branch
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
