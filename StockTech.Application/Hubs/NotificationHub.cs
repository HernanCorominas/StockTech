using Microsoft.AspNetCore.SignalR;

namespace StockTech.Application.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var branchId = Context.User?.FindFirst("BranchId")?.Value;
        if (!string.IsNullOrEmpty(branchId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, branchId);
        }
        await base.OnConnectedAsync();
    }

    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
