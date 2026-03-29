using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly StockTechDbContext _ctx;

    public ActivityLogRepository(StockTechDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task AddAsync(ActivityLog log)
    {
        await _ctx.ActivityLogs.AddAsync(log);
    }

    public async Task<IEnumerable<ActivityLog>> GetRecentActivityAsync(int count)
    {
        return await _ctx.ActivityLogs
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetActivityByBranchAsync(Guid branchId, int count)
    {
        return await _ctx.ActivityLogs
            .Include(x => x.User)
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IEnumerable<ActivityLog> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? branchId, DateTime? start, DateTime? end)
    {
        var query = _ctx.ActivityLogs
            .Include(x => x.User)
            .AsNoTracking();

        if (branchId.HasValue && branchId != Guid.Empty)
        {
            query = query.Where(x => x.BranchId == branchId.Value);
        }

        if (start.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= start.Value);
        }

        if (end.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= end.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
