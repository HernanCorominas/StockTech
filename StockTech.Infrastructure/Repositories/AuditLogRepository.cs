using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly StockTechDbContext _ctx;

    public AuditLogRepository(StockTechDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count)
    {
        return await _ctx.AuditLogs
            .AsNoTracking()
            .Include(l => l.Branch)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByFiltersAsync(string? tableName, string? action, DateTime? start, DateTime? end, Guid? branchId)
    {
        var query = _ctx.AuditLogs
            .AsNoTracking()
            .Include(l => l.Branch)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(l => l.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(tableName))
            query = query.Where(l => l.TableName == tableName);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action == action);

        if (start.HasValue)
        {
            var utcStart = DateTime.SpecifyKind(start.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt >= utcStart);
        }

        if (end.HasValue)
        {
            var utcEnd = DateTime.SpecifyKind(end.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt <= utcEnd);
        }

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? tableName, string? action, DateTime? start, DateTime? end, Guid? branchId)
    {
        var query = _ctx.AuditLogs
            .AsNoTracking()
            .Include(l => l.Branch)
            .AsQueryable();

        if (branchId.HasValue && branchId != Guid.Empty)
            query = query.Where(l => l.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(tableName))
            query = query.Where(l => l.TableName == tableName);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action == action);

        if (start.HasValue)
        {
            var utcStart = DateTime.SpecifyKind(start.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt >= utcStart);
        }

        if (end.HasValue)
        {
            var utcEnd = DateTime.SpecifyKind(end.Value, DateTimeKind.Utc);
            query = query.Where(l => l.CreatedAt <= utcEnd);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
