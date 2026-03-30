using StockTech.Domain.Entities;

namespace StockTech.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationToUserAsync(Guid userId, string message, string type = "Info");
    Task SendNotificationToBranchAsync(Guid branchId, string message, string type = "Info");
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
}
