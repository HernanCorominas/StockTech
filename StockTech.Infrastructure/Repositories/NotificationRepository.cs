using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly StockTechDbContext _ctx;
    public NotificationRepository(StockTechDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int count = 20)
    {
        return await _ctx.Set<Notification>()
            .Where(n => n.UserId == userId || (n.UserId == null && n.BranchId == null))
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetByBranchIdAsync(Guid branchId, int count = 20)
    {
        return await _ctx.Set<Notification>()
            .Where(n => n.BranchId == branchId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(Notification notification)
    {
        await _ctx.Set<Notification>().AddAsync(notification);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var n = await _ctx.Set<Notification>().FindAsync(notificationId);
        if (n != null) n.IsRead = true;
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _ctx.Set<Notification>()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
        unread.ForEach(n => n.IsRead = true);
    }
}
