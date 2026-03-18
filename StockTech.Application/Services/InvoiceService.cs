using StockTech.Application.DTOs.Invoices;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _uow;

    public InvoiceService(IUnitOfWork uow) => _uow = uow;

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

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        // Business rule guards (Onion: domain integrity enforced in service layer)
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("La factura debe tener al menos un item.");

        if (dto.Items.Any(i => i.Quantity <= 0))
            throw new ArgumentException("La cantidad de cada item debe ser mayor a cero.");

        if (dto.TaxRate < 0 || dto.TaxRate > 1)
            throw new ArgumentException("El tax rate debe estar entre 0 y 1 (ej: 0.18 para 18%).");

        var client = await _uow.Clients.GetByIdAsync(dto.ClientId)
            ?? throw new ArgumentException("Client not found");

        var invoice = new Invoice
        {
            InvoiceNumber = await GenerateInvoiceNumberAsync(),
            ClientId = dto.ClientId,
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
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });

            // Decrement stock
            product.Stock -= itemDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
            _uow.Products.Update(product);
        }

        invoice.Subtotal = subtotal;
        invoice.TaxAmount = subtotal * dto.TaxRate;
        invoice.Total = subtotal + invoice.TaxAmount;

        await _uow.Invoices.AddAsync(invoice);
        await _uow.CommitAsync();

        // Reload to get navigation properties
        var created = await _uow.Invoices.GetByIdAsync(invoice.Id);
        return Map(created!);
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
        i.Client?.Name ?? string.Empty,
        i.Client?.Document ?? string.Empty,
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
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.LineTotal
        )).ToList(),
        i.CreatedAt
    );
}
