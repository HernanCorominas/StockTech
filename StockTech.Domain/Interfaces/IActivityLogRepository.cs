using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log);
    Task<IEnumerable<ActivityLog>> GetRecentActivityAsync(int count);
    Task<IEnumerable<ActivityLog>> GetActivityByBranchAsync(Guid branchId, int count);
    Task<(IEnumerable<ActivityLog> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? branchId, DateTime? start, DateTime? end);
}
