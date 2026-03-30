using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int count = 20);
    Task<IEnumerable<Notification>> GetByBranchIdAsync(Guid branchId, int count = 20);
    Task AddAsync(Notification notification);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
}
