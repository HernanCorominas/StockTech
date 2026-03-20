using System;

namespace StockTech.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Insert, Update, Delete
    public string? KeyValues { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? User { get; set; }
}
