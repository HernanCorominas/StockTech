using StockTech.Application.DTOs.Products;
using StockTech.Application.DTOs.Invoices;

namespace StockTech.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportProductsToExcelAsync(IEnumerable<ProductDto> products);
    Task<byte[]> ExportInvoicesToExcelAsync(IEnumerable<InvoiceDto> invoices);
    Task<byte[]> ExportProductsToPdfAsync(IEnumerable<ProductDto> products);
    Task<byte[]> ExportInvoicesToPdfAsync(IEnumerable<InvoiceDto> invoices);
}
