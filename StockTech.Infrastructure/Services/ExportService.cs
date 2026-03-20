using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockTech.Application.DTOs.Invoices;
using StockTech.Application.DTOs.Products;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Services;

public class ExportService : IExportService
{
    static ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportProductsToExcelAsync(IEnumerable<ProductDto> products)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Productos");
        
        sheet.Cell(1, 1).Value = "Nombre";
        sheet.Cell(1, 2).Value = "SKU";
        sheet.Cell(1, 3).Value = "Categoría";
        sheet.Cell(1, 4).Value = "Precio";
        sheet.Cell(1, 5).Value = "Stock";
        sheet.Cell(1, 6).Value = "Stock Mín.";
        
        var header = sheet.Row(1);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        header.Style.Font.FontColor = XLColor.White;

        int row = 2;
        foreach (var p in products)
        {
            sheet.Cell(row, 1).Value = p.Name;
            sheet.Cell(row, 2).Value = p.SKU ?? string.Empty;
            sheet.Cell(row, 3).Value = p.Category ?? string.Empty;
            sheet.Cell(row, 4).Value = (double)p.Price;
            sheet.Cell(row, 5).Value = p.Stock;
            sheet.Cell(row, 6).Value = p.MinStock;
            row++;
        }
        
        sheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    public async Task<byte[]> ExportInvoicesToExcelAsync(IEnumerable<InvoiceDto> invoices)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Facturas");
        
        sheet.Cell(1, 1).Value = "# Factura";
        sheet.Cell(1, 2).Value = "Fecha";
        sheet.Cell(1, 3).Value = "Cliente";
        sheet.Cell(1, 4).Value = "Total";
        sheet.Cell(1, 5).Value = "Estado";
        
        var header = sheet.Row(1);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
        header.Style.Font.FontColor = XLColor.White;

        int row = 2;
        foreach (var i in invoices)
        {
            sheet.Cell(row, 1).Value = i.InvoiceNumber;
            sheet.Cell(row, 2).Value = i.InvoiceDate.ToString("dd/MM/yyyy");
            sheet.Cell(row, 3).Value = i.ClientName ?? "N/A";
            sheet.Cell(row, 4).Value = (double)i.Total;
            sheet.Cell(row, 5).Value = i.Status;
            row++;
        }
        
        sheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    public async Task<byte[]> ExportProductsToPdfAsync(IEnumerable<ProductDto> products)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("StockTech – Reporte de Productos").FontSize(20).Bold().FontColor(Colors.Indigo.Medium);
                
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).Padding(5).Text("Producto").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("SKU").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("Precio").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("Stock").Bold();
                    });

                    foreach (var p in products)
                    {
                        table.Cell().Padding(5).Text(p.Name);
                        table.Cell().Padding(5).Text(p.SKU ?? "—");
                        table.Cell().Padding(5).Text(p.Price.ToString("C"));
                        table.Cell().Padding(5).Text(p.Stock.ToString());
                    }
                });
                
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    public async Task<byte[]> ExportInvoicesToPdfAsync(IEnumerable<InvoiceDto> invoices)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("StockTech – Reporte de Facturas").FontSize(20).Bold().FontColor(Colors.Indigo.Medium);
                
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).Padding(5).Text("# Factura").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("Fecha").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("Cliente").Bold();
                        header.Cell().BorderBottom(1).Padding(5).Text("Total").Bold();
                    });

                    foreach (var i in invoices)
                    {
                        table.Cell().Padding(5).Text(i.InvoiceNumber);
                        table.Cell().Padding(5).Text(i.InvoiceDate.ToString("dd/MM/yyyy"));
                        table.Cell().Padding(5).Text(i.ClientName ?? "—");
                        table.Cell().Padding(5).Text(i.Total.ToString("C"));
                    }
                });
            });
        }).GeneratePdf();
    }
}
