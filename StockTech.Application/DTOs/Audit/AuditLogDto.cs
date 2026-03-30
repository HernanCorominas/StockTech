namespace StockTech.Application.DTOs.Audit;

public record AuditLogDto(
    Guid Id,
    string EntityName,
    string Action,
    string EntityId,
    string? OldValues,
    string? NewValues,
    string? UserId,
    DateTime CreatedAt,
    Guid? BranchId,
    string? BranchName
);
