using StockTech.Application.DTOs.Audit;
using StockTech.Application.DTOs.Common;

namespace StockTech.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetRecentLogsAsync(int count = 50);
    Task<PagedResult<AuditLogDto>> GetLogsByFiltersAsync(int page, int pageSize, string? tableName, string? action, DateTime? start, DateTime? end);
    // Removemos LogActionAsync manual ya que ahora es automático en DbContext
}
