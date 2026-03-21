namespace StockTech.Application.DTOs.Reports;

public record ReportFilterDto(DateTime From, DateTime To, string? BranchId = null);

public record SalesReportItemDto(
    string InvoiceNumber,
    string ClientName,
    DateTime InvoiceDate,
    decimal Total
);

public record PurchaseReportItemDto(
    string PurchaseNumber,
    string Supplier,
    DateTime PurchaseDate,
    decimal Total
);

public record ReportSummaryDto(
    decimal TotalSales,
    decimal TotalPurchases,
    decimal NetProfit,
    List<SalesReportItemDto> Sales,
    List<PurchaseReportItemDto> Purchases
);
