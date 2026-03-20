namespace StockTech.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string message);
    Task SendLowStockAlertAsync(string productName, int currentStock);
}
