using StockTech.Application.DTOs.Purchases;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;
using StockTech.Domain.Enums;

namespace StockTech.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _uow;
    private readonly IInventoryService _inventoryService;

    public PurchaseService(IUnitOfWork uow, IInventoryService inventoryService)
    {
        _uow = uow;
        _inventoryService = inventoryService;
    }

    public async Task<IEnumerable<PurchaseDto>> GetAllAsync()
    {
        var purchases = await _uow.Purchases.GetAllAsync();
        return purchases.Select(Map);
    }

    public async Task<PurchaseDto?> GetByIdAsync(Guid id)
    {
        var purchase = await _uow.Purchases.GetByIdAsync(id);
        return purchase is null ? null : Map(purchase);
    }

    public async Task<PurchaseDto> CreateAsync(CreatePurchaseDto dto)
    {
        var purchase = new Purchase
        {
            PurchaseNumber = await GeneratePurchaseNumberAsync(),
            SupplierId = dto.SupplierId,
            BranchId = dto.BranchId,
            Notes = dto.Notes,
            PurchaseDate = DateTime.UtcNow
        };

        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _uow.Products.GetByIdAsync(itemDto.ProductId)
                ?? throw new ArgumentException($"Product {itemDto.ProductId} not found");

            var lineTotal = itemDto.UnitCost * itemDto.Quantity;
            total += lineTotal;

            purchase.Items.Add(new PurchaseItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitCost = itemDto.UnitCost,
                LineTotal = lineTotal
            });

            // Log inventory transaction (Kardex)
            await _inventoryService.LogTransactionAsync(
                product.Id, 
                itemDto.Quantity, 
                TransactionType.Purchase, 
                purchase.PurchaseNumber,
                purchaseId: purchase.Id);

            // Increment stock
            product.Stock += itemDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
            _uow.Products.Update(product);
        }

        purchase.Total = total;
        await _uow.Purchases.AddAsync(purchase);
        await _uow.CommitAsync();

        var created = await _uow.Purchases.GetByIdAsync(purchase.Id);
        return Map(created!);
    }

    private async Task<string> GeneratePurchaseNumberAsync()
    {
        var all = await _uow.Purchases.GetAllAsync();
        int next = all.Count() + 1;
        return $"COM-{next:D6}";
    }

    private static PurchaseDto Map(Purchase p) => new(
        p.Id,
        p.PurchaseNumber,
        p.SupplierId,
        p.Supplier?.Name ?? "Unknown",
        p.BranchId,
        p.Branch?.Name,
        p.PurchaseDate,
        p.Total,
        p.Notes,
        p.Items.Select(item => new PurchaseItemDto(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitCost,
            item.LineTotal
        )).ToList(),
        p.CreatedAt
    );
}
