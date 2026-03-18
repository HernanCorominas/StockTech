using StockTech.Application.DTOs.Reports;

namespace StockTech.Application.Interfaces;

public interface IExcelExportService
{
    Task<byte[]> ExportReportToExcelAsync(ReportSummaryDto summary, DateTime from, DateTime to);
}
