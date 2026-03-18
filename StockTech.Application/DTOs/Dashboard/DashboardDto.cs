namespace StockTech.Application.DTOs.Dashboard;

public record DashboardDto(
    decimal TotalSales,
    decimal TotalPurchases,
    int TotalInvoices,
    int TotalPurchasesCount,
    int TotalClients,
    int TotalProducts,
    int LowStockProducts,
    decimal Profit,
    List<MonthlySummaryDto> MonthlySales,
    List<MonthlySummaryDto> MonthlyPurchases,
    List<TopProductDto> TopProducts
);

public record MonthlySummaryDto(string Month, decimal Total);
public record TopProductDto(string ProductName, int QuantitySold, decimal Revenue);
