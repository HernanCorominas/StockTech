using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count);
    Task<IEnumerable<AuditLog>> GetLogsByFiltersAsync(string? tableName, string? action, DateTime? start, DateTime? end, Guid? branchId);
    Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? tableName, string? action, DateTime? start, DateTime? end, Guid? branchId);
}
