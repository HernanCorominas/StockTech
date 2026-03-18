using StockTech.Application.DTOs.Reports;

namespace StockTech.Application.Interfaces;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync(ReportFilterDto filter);
    Task<byte[]> ExportToExcelAsync(ReportFilterDto filter);
}
