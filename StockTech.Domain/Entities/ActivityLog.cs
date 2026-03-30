using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class ActivityLog : BaseEntity, ITenantEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    
    public string Message { get; set; } = string.Empty; // Human readable
    public string Category { get; set; } = string.Empty; // Inventory, Sales, Auth, etc.
    public string? Details { get; set; } // JSON or extra info
    
    public Guid? CorrelationId { get; set; }
    
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
}
