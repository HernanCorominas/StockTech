using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class AuditLog : BaseEntity, ITenantEntity
{
    public string TableName { get; set; } = null!;
    public string KeyValues { get; set; } = null!;
    public string Action { get; set; } = null!;     // Create, Update, Delete
    public string? UserId { get; set; }             // ID del usuario
    public string? OldValues { get; set; }          // JSON del estado anterior
    public string? NewValues { get; set; }          // JSON del estado nuevo
    public Guid? BranchId { get; set; }
    public virtual Branch? Branch { get; set; }
}
