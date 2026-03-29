using StockTech.Application.DTOs.Audit;
using StockTech.Application.DTOs.Common;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogs;
    private readonly ITenantService _tenantService;

    public AuditLogService(IAuditLogRepository auditLogs, ITenantService tenantService)
    {
        _auditLogs = auditLogs;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<AuditLogDto>> GetRecentLogsAsync(int count = 50)
    {
        var logs = await _auditLogs.GetRecentLogsAsync(count);

        return logs.Select(Map);
    }

    public async Task<PagedResult<AuditLogDto>> GetLogsByFiltersAsync(int page, int pageSize, string? tableName, string? action, DateTime? start, DateTime? end)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await _auditLogs.GetPagedAsync(page, pageSize, tableName, action, start, end, _tenantService.BranchId);

        return new PagedResult<AuditLogDto>
        {
            Items = items.Select(Map),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    private static AuditLogDto Map(AuditLog l) => new(
        l.Id, 
        l.TableName, 
        l.Action, 
        l.KeyValues, 
        l.OldValues, 
        l.NewValues, 
        l.UserId, 
        l.CreatedAt, 
        l.BranchId, 
        l.Branch?.Name
    );
}
