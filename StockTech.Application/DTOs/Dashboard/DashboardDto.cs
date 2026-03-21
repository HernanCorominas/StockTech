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
    List<TopProductDto> TopProducts,
    List<CategoryStockDto> CategoryDistribution,
    List<BranchSalesDto> BranchSales
);

public record MonthlySummaryDto(string Month, decimal Total);
public record TopProductDto(string ProductName, int QuantitySold, decimal Revenue);
public record CategoryStockDto(string Category, int StockCount);
public record BranchSalesDto(string BranchName, decimal TotalSales);
