using StockTech.Application.DTOs.Reports;
using StockTech.Application.Interfaces;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    private readonly IExcelExportService _excelExporter;

    public ReportService(IUnitOfWork uow, IExcelExportService excelExporter)
    {
        _uow = uow;
        _excelExporter = excelExporter;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(ReportFilterDto filter)
    {
        var invoices = (await _uow.Invoices.GetByDateRangeAsync(filter.From, filter.To)).ToList();
        var purchases = (await _uow.Purchases.GetByDateRangeAsync(filter.From, filter.To)).ToList();

        var totalSales = invoices.Sum(i => i.Total);
        var totalPurchases = purchases.Sum(p => p.Total);

        var salesItems = invoices.Select(i => new SalesReportItemDto(
            i.InvoiceNumber,
            i.Client?.Name ?? string.Empty,
            i.InvoiceDate,
            i.Total
        )).ToList();

        var purchaseItems = purchases.Select(p => new PurchaseReportItemDto(
            p.PurchaseNumber,
            p.Supplier?.Name ?? "Unknown",
            p.PurchaseDate,
            p.Total
        )).ToList();

        return new ReportSummaryDto(
            TotalSales: totalSales,
            TotalPurchases: totalPurchases,
            NetProfit: totalSales - totalPurchases,
            Sales: salesItems,
            Purchases: purchaseItems
        );
    }

    public async Task<byte[]> ExportToExcelAsync(ReportFilterDto filter)
    {
        var summary = await GetSummaryAsync(filter);
        return await _excelExporter.ExportReportToExcelAsync(summary, filter.From, filter.To);
    }
}
