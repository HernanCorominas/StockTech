using StockTech.Application.DTOs.Common;
using StockTech.Application.DTOs.Invoices;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Domain.Enums;
using System.Linq;
using System.Collections.Generic;

namespace StockTech.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _uow;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IDocumentService _documentService;
    private readonly IStorageService _storageService;

    public InvoiceService(
        IUnitOfWork uow, 
        IInventoryMovementService inventoryMovementService,
        IDocumentService documentService,
        IStorageService storageService)
    {
        _uow = uow;
        _inventoryMovementService = inventoryMovementService;
        _documentService = documentService;
        _storageService = storageService;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _uow.Invoices.GetAllAsync();
        return invoices.Select(Map);
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var invoice = await _uow.Invoices.GetByIdAsync(id);
        return invoice is null ? null : Map(invoice);
    }

    public async Task<PagedResult<InvoiceDto>> GetPagedAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        
        var result = await _uow.Invoices.GetPagedAsync(page, pageSize, search);
        return new PagedResult<InvoiceDto>
        {
            Items = result.Items.Select(Map),
            TotalCount = result.TotalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        Client? client = null;

        // 1. Resolve Client
        if (dto.ClientId.HasValue)
        {
            client = await _uow.Clients.GetByIdAsync(dto.ClientId.Value)
                ?? throw new ArgumentException("Client not found");
        }
        else if (!string.IsNullOrWhiteSpace(dto.CustomerDocument))
        {
            // Auto-resolve or Auto-register (Optimized)
            client = await _uow.Clients.GetByDocumentAsync(dto.CustomerDocument);

            if (client == null && !string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                // Create new client on the fly
                client = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = dto.CustomerName,
                    Document = dto.CustomerDocument,
                    BranchId = dto.BranchId, // Linked to the same branch as the sale
                    IsActive = true
                };
                await _uow.Clients.AddAsync(client);
            }
        }

        var invoice = new Invoice
        {
            InvoiceNumber = await GenerateInvoiceNumberAsync(),
            ClientId = client?.Id,
            BranchId = dto.BranchId ?? Guid.Empty, // Defaulting to Empty for now, will fix in Sprint 2
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            InvoiceDate = DateTime.UtcNow
        };

        decimal subtotal = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _uow.Products.GetByIdAsync(itemDto.ProductId)
                ?? throw new ArgumentException($"Product {itemDto.ProductId} not found");

            if (product.Stock < itemDto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

            var lineTotal = product.Price * itemDto.Quantity;
            subtotal += lineTotal;

            invoice.Items.Add(new InvoiceItem
            {
                ProductId = product.Id,
                VariantId = itemDto.VariantId,
                Quantity = (int)itemDto.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });

            // Log inventory transaction (Kardex) - Now handles Stock update
            await _inventoryMovementService.LogTransactionAsync(
                product.Id, 
                itemDto.Quantity, 
                TransactionType.Sale, 
                invoice.InvoiceNumber,
                invoiceId: invoice.Id,
                variantId: itemDto.VariantId);
        }

        invoice.Subtotal = subtotal;
        invoice.TaxAmount = subtotal * dto.TaxRate;
        invoice.Total = subtotal + invoice.TaxAmount;

        await _uow.Invoices.AddAsync(invoice);

        // 5. Activity Log (Human Readable)
        var activityLog = new ActivityLog
        {
            Id = Guid.NewGuid(),
            BranchId = invoice.BranchId,
            Message = $"Venta registrada: {invoice.InvoiceNumber} a {client?.Name ?? "Consumidor Final"} por {invoice.Total:C}",
            Category = "Sales",
            CreatedAt = DateTime.UtcNow
        };
        await _uow.ActivityLogs.AddAsync(activityLog);

        await _uow.CommitAsync();

        // Reload to get navigation properties
        var created = await _uow.Invoices.GetByIdAsync(invoice.Id);
        return Map(created!);
    }

    public async Task CancelAsync(Guid id)
    {
        var invoice = await _uow.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Invoice {id} not found");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Invoice is already cancelled");

        // Reverse stock for each line item
        foreach (var item in invoice.Items)
        {
            // Negative quantity = adjustment that restores stock (adds back)
            await _inventoryMovementService.LogTransactionAsync(
                item.ProductId,
                -item.Quantity,   // negative triggers reversal in LogTransactionAsync
                TransactionType.Adjustment,
                $"CANCEL-{invoice.InvoiceNumber}",
                variantId: item.VariantId);
        }

        invoice.Status = InvoiceStatus.Cancelled;
        invoice.UpdatedAt = DateTime.UtcNow;
        _uow.Invoices.Update(invoice);

        var activityLog = new ActivityLog
        {
            Id = Guid.NewGuid(),
            BranchId = invoice.BranchId,
            Message = $"Factura anulada: {invoice.InvoiceNumber}. Stock revertido.",
            Category = "Sales",
            CreatedAt = DateTime.UtcNow
        };
        await _uow.ActivityLogs.AddAsync(activityLog);

        await _uow.CommitAsync();
    }

    public async Task<string> GetInvoicePdfAsync(Guid id)
    {
        var invoice = await _uow.Invoices.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Factura no encontrada");

        var dto = Map(invoice);
        var pdfBytes = await _documentService.GenerateInvoicePdfAsync(dto);
        
        var fileName = $"Factura_{invoice.InvoiceNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var url = await _storageService.UploadFileAsync(pdfBytes, fileName, "application/pdf", "invoices");

        return url;
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var all = await _uow.Invoices.GetAllAsync();
        int next = all.Count() + 1;
        return $"FAC-{next:D6}";
    }

    private static InvoiceDto Map(Invoice i) => new(
        i.Id,
        i.InvoiceNumber,
        i.ClientId,
        i.Client?.Name ?? "Consumidor Final",
        i.Client?.Document ?? "RNC Genérico",
        i.BranchId,
        i.Branch?.Name,
        i.InvoiceDate,
        i.Subtotal,
        i.TaxRate,
        i.TaxAmount,
        i.Total,
        i.Status,
        i.Status.ToString(),
        i.Notes,
        i.Items.Select(item => new InvoiceItemDto(
            item.Id,
            item.ProductId,
            item.VariantId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.LineTotal
        )).ToList(),
        i.CreatedAt
    );
}
