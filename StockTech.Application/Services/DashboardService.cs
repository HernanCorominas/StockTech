using StockTech.Application.DTOs.Dashboard;
using StockTech.Application.Interfaces;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardDto> GetMetricsAsync()
    {
        var invoices = (await _uow.Invoices.GetAllAsync()).ToList();
        var purchases = (await _uow.Purchases.GetAllAsync()).ToList();
        var clients = (await _uow.Clients.GetAllAsync()).ToList();
        var products = (await _uow.Products.GetAllAsync()).ToList();

        var totalSales = invoices.Sum(i => i.Total);
        var totalPurchases = purchases.Sum(p => p.Total);

        // Monthly sales (last 6 months)
        var monthlySales = invoices
            .GroupBy(i => i.InvoiceDate.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .TakeLast(6)
            .Select(g => new MonthlySummaryDto(g.Key, g.Sum(i => i.Total)))
            .ToList();

        var monthlyPurchases = purchases
            .GroupBy(p => p.PurchaseDate.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .TakeLast(6)
            .Select(g => new MonthlySummaryDto(g.Key, g.Sum(p => p.Total)))
            .ToList();

        // Top 5 products by revenue
        var topProducts = invoices
            .SelectMany(i => i.Items)
            .GroupBy(item => item.Product?.Name ?? "Unknown")
            .Select(g => new TopProductDto(g.Key, g.Sum(i => i.Quantity), g.Sum(i => i.LineTotal)))
            .OrderByDescending(x => x.Revenue)
            .Take(5)
            .ToList();

        // Category distribution
        var categoryDistribution = products
            .GroupBy(p => p.Category ?? "Sin Categoría")
            .Select(g => new CategoryStockDto(g.Key, g.Sum(p => p.Stock)))
            .ToList();

        return new DashboardDto(
            TotalSales: totalSales,
            TotalPurchases: totalPurchases,
            TotalInvoices: invoices.Count,
            TotalPurchasesCount: purchases.Count,
            TotalClients: clients.Count,
            TotalProducts: products.Count,
            LowStockProducts: products.Count(p => p.Stock <= p.MinStock),
            Profit: totalSales - totalPurchases,
            MonthlySales: monthlySales,
            MonthlyPurchases: monthlyPurchases,
            TopProducts: topProducts,
            CategoryDistribution: categoryDistribution
        );
    }
}
