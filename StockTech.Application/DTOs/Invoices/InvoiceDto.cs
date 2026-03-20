using System.ComponentModel.DataAnnotations;
using StockTech.Domain.Enums;

namespace StockTech.Application.DTOs.Invoices;

public record CreateInvoiceItemDto(
    [Required] Guid ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")] int Quantity
);

public record CreateInvoiceDto(
    [Required] Guid ClientId,
    Guid? BranchId,
    [Required, MinLength(1, ErrorMessage = "La factura debe tener al menos un item.")] List<CreateInvoiceItemDto> Items,
    [Range(0, 1, ErrorMessage = "Tax rate debe estar entre 0 y 1.")] decimal TaxRate,
    string? Notes
);


public record InvoiceItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    Guid ClientId,
    string ClientName,
    string ClientDocument,
    Guid? BranchId,
    string? BranchName,
    DateTime InvoiceDate,
    decimal Subtotal,
    decimal TaxRate,
    decimal TaxAmount,
    decimal Total,
    InvoiceStatus Status,
    string StatusName,
    string? Notes,
    List<InvoiceItemDto> Items,
    DateTime CreatedAt
);
