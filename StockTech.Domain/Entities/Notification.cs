using System;
using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class Notification : BaseEntity, ITenantEntity
{
    public string Message { get; set; } = null!;
    public string? Type { get; set; } // "LowStock", "System", "Info"
    public bool IsRead { get; set; }
    public Guid? UserId { get; set; } // Targeted user (optional)
    public Guid? BranchId { get; set; } // Targeted branch (optional)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
