using System.ComponentModel.DataAnnotations;
using StockTech.Domain.Enums;
using System.Collections.Generic;

namespace StockTech.Application.DTOs.Invoices;

public record CreateInvoiceItemDto(
    [Required] Guid ProductId,
    Guid? VariantId,
    [Range(0.001, 1000000, ErrorMessage = "La cantidad debe ser mayor a cero.")] decimal Quantity
);

public record CreateInvoiceDto(
    Guid? ClientId,
    string? CustomerName,    // For "on-the-fly" registration
    string? CustomerDocument, // For "on-the-fly" registration
    Guid? BranchId,
    [Required, MinLength(1, ErrorMessage = "La factura debe tener al menos un item.")] List<CreateInvoiceItemDto> Items,
    [Range(0, 1, ErrorMessage = "Tax rate debe estar entre 0 y 1.")] decimal TaxRate,
    string? Notes
);


public record InvoiceItemDto(
    Guid Id,
    Guid ProductId,
    Guid? VariantId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    Guid? ClientId,
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
