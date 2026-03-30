using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using StockTech.Application.DTOs.Invoices;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    public DocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(InvoiceDto invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("StockTech").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text("Soluciones Tecnológicas ERP").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"FACTURA #{invoice.InvoiceNumber}").FontSize(18).SemiBold();
                        col.Item().Text($"Fecha: {invoice.InvoiceDate:dd/MM/yyyy}").FontSize(9);
                        col.Item().Text($"Sucursal: {invoice.BranchName ?? "Sede Central"}").FontSize(9);
                    });
                });

                page.Content().PaddingVertical(25).Column(col =>
                {
                    // Cliente Info
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("CLIENTE").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(5).Text(invoice.ClientName).FontSize(12).SemiBold();
                            c.Item().Text($"RNC/Cédula: {invoice.ClientDocument}").FontSize(9);
                        });
                    });

                    col.Item().PaddingTop(25).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Precio");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var item in invoice.Items)
                        {
                            table.Cell().Element(ItemStyle).Text(item.ProductName);
                            table.Cell().Element(ItemStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                            table.Cell().Element(ItemStyle).AlignRight().Text(item.UnitPrice.ToString("C"));
                            table.Cell().Element(ItemStyle).AlignRight().Text(item.LineTotal.ToString("C"));

                            static IContainer ItemStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                            }
                        }
                    });

                    // Totales
                    col.Item().AlignRight().PaddingTop(20).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text("Subtotal:").FontSize(9);
                            r.ConstantItem(80).AlignRight().Text(invoice.Subtotal.ToString("C")).FontSize(9);
                        });
                        c.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text($"Impuestos ({invoice.TaxRate:P0}):").FontSize(9);
                            r.ConstantItem(80).AlignRight().Text(invoice.TaxAmount.ToString("C")).FontSize(9);
                        });
                        c.Item().PaddingTop(5).Row(r =>
                        {
                            r.ConstantItem(100).Text("TOTAL:").FontSize(14).SemiBold();
                            r.ConstantItem(80).AlignRight().Text(invoice.Total.ToString("C")).FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                        });
                    });

                    if (!string.IsNullOrEmpty(invoice.Notes))
                    {
                        col.Item().PaddingTop(30).Column(c =>
                        {
                            c.Item().Text("NOTAS / COMENTARIOS").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().Text(invoice.Notes).FontSize(9).Italic();
                        });
                    }
                });

                page.Footer().AlignCenter().Column(c =>
                {
                    c.Item().Text("Gracias por su preferencia").FontSize(9).FontColor(Colors.Grey.Medium);
                    c.Item().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return await Task.FromResult(stream.ToArray());
    }

    public Task<byte[]> GenerateInventoryReportPdfAsync(IEnumerable<dynamic> data)
    {
        // Implementación futura
        return Task.FromResult(Array.Empty<byte>());
    }
}
