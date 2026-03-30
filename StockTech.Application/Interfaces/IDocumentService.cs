using StockTech.Application.DTOs.Invoices;

namespace StockTech.Application.Interfaces;

public interface IDocumentService
{
    Task<byte[]> GenerateInvoicePdfAsync(InvoiceDto invoice);
    Task<byte[]> GenerateInventoryReportPdfAsync(IEnumerable<dynamic> data);
}
