using ClosedXML.Excel;
using StockTech.Application.DTOs.Reports;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public Task<byte[]> ExportReportToExcelAsync(ReportSummaryDto summary, DateTime from, DateTime to)
    {
        using var workbook = new XLWorkbook();

        // Sales Sheet
        var salesSheet = workbook.Worksheets.Add("Ventas");
        salesSheet.Cell(1, 1).Value = "# Factura";
        salesSheet.Cell(1, 2).Value = "Cliente";
        salesSheet.Cell(1, 3).Value = "Fecha";
        salesSheet.Cell(1, 4).Value = "Total (RD$)";
        salesSheet.Row(1).Style.Font.Bold = true;
        salesSheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        salesSheet.Row(1).Style.Font.FontColor = XLColor.White;

        int row = 2;
        foreach (var s in summary.Sales)
        {
            salesSheet.Cell(row, 1).Value = s.InvoiceNumber;
            salesSheet.Cell(row, 2).Value = s.ClientName;
            salesSheet.Cell(row, 3).Value = s.InvoiceDate.ToString("dd/MM/yyyy");
            salesSheet.Cell(row, 4).Value = s.Total;
            row++;
        }
        if (summary.Sales.Count > 0)
        {
            salesSheet.Cell(row, 3).Value = "TOTAL:";
            salesSheet.Cell(row, 4).Value = summary.TotalSales;
            salesSheet.Range(row, 3, row, 4).Style.Font.Bold = true;
        }
        salesSheet.Columns().AdjustToContents();

        // Purchases Sheet
        var purchasesSheet = workbook.Worksheets.Add("Compras");
        purchasesSheet.Cell(1, 1).Value = "# Compra";
        purchasesSheet.Cell(1, 2).Value = "Proveedor";
        purchasesSheet.Cell(1, 3).Value = "Fecha";
        purchasesSheet.Cell(1, 4).Value = "Total (RD$)";
        purchasesSheet.Row(1).Style.Font.Bold = true;
        purchasesSheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        purchasesSheet.Row(1).Style.Font.FontColor = XLColor.White;

        row = 2;
        foreach (var p in summary.Purchases)
        {
            purchasesSheet.Cell(row, 1).Value = p.PurchaseNumber;
            purchasesSheet.Cell(row, 2).Value = p.Supplier;
            purchasesSheet.Cell(row, 3).Value = p.PurchaseDate.ToString("dd/MM/yyyy");
            purchasesSheet.Cell(row, 4).Value = p.Total;
            row++;
        }
        if (summary.Purchases.Count > 0)
        {
            purchasesSheet.Cell(row, 3).Value = "TOTAL:";
            purchasesSheet.Cell(row, 4).Value = summary.TotalPurchases;
            purchasesSheet.Range(row, 3, row, 4).Style.Font.Bold = true;
        }
        purchasesSheet.Columns().AdjustToContents();

        // Summary Sheet
        var summarySheet = workbook.Worksheets.Add("Resumen");
        summarySheet.Cell(1, 1).Value = "StockTech – Reporte";
        summarySheet.Cell(1, 1).Style.Font.Bold = true;
        summarySheet.Cell(1, 1).Style.Font.FontSize = 14;
        summarySheet.Cell(2, 1).Value = $"Período: {from:dd/MM/yyyy} – {to:dd/MM/yyyy}";
        summarySheet.Cell(4, 1).Value = "Métrica";
        summarySheet.Cell(4, 2).Value = "Valor (RD$)";
        summarySheet.Row(4).Style.Font.Bold = true;
        summarySheet.Cell(5, 1).Value = "Total Ventas";
        summarySheet.Cell(5, 2).Value = summary.TotalSales;
        summarySheet.Cell(6, 1).Value = "Total Compras";
        summarySheet.Cell(6, 2).Value = summary.TotalPurchases;
        summarySheet.Cell(7, 1).Value = "Ganancia Neta";
        summarySheet.Cell(7, 2).Value = summary.NetProfit;
        summarySheet.Cell(7, 2).Style.Font.Bold = true;
        summarySheet.Cell(7, 2).Style.Font.FontColor = summary.NetProfit >= 0
            ? XLColor.FromHtml("#16A34A")
            : XLColor.FromHtml("#DC2626");
        summarySheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
