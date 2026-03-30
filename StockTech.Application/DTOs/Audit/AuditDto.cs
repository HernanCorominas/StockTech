namespace StockTech.Application.DTOs.Audit;

public record AuditDto(
    Guid Id,
    string EntityName,
    string EntityId,
    string Action,
    string? UserId,
    string? Username,
    DateTime Timestamp,
    string? OldValues, // JSON
    string? NewValues, // JSON
    string? ChangedColumns, // JSON
    Guid? BranchId
);
