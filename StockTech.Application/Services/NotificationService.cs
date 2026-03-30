
using Microsoft.AspNetCore.SignalR;
using StockTech.Application.Hubs;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IUnitOfWork uow, IHubContext<NotificationHub> hubContext)
    {
        _uow = uow;
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, string message, string type = "Info")
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification);
        await _uow.CommitAsync();

        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
    }

    public async Task SendNotificationToBranchAsync(Guid branchId, string message, string type = "Info")
    {
        var notification = new Notification
        {
            BranchId = branchId,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification);
        await _uow.CommitAsync();

        await _hubContext.Clients.Group(branchId.ToString()).SendAsync("ReceiveNotification", message);
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId)
    {
        return await _uow.Notifications.GetByUserIdAsync(userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        await _uow.Notifications.MarkAsReadAsync(notificationId);
        await _uow.CommitAsync();
    }
}
