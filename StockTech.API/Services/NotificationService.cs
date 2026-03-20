using Microsoft.AspNetCore.SignalR;
using StockTech.API.Hubs;
using StockTech.Application.Interfaces;

namespace StockTech.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task SendLowStockAlertAsync(string productName, int currentStock)
    {
        var message = $"Alerta: El stock de '{productName}' es bajo ({currentStock} unidades).";
        await SendNotificationAsync(message);
    }
}
