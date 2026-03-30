using Microsoft.EntityFrameworkCore;
using StockTech.Application.DTOs.Inventory;
using StockTech.Application.DTOs.Purchases;
using StockTech.Application.Interfaces;
using StockTech.Application.Services;
using StockTech.Domain.Entities;
using StockTech.Domain.Enums;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class InventoryMovementService : IInventoryMovementService
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;
    private readonly ITenantService _tenantService;

    public InventoryMovementService(IUnitOfWork uow, INotificationService notificationService, ITenantService tenantService)
    {
        _uow = uow;
        _notificationService = notificationService;
        _tenantService = tenantService;
    }

    public async Task<PurchaseDto?> ProcessEntryAsync(StockEntryRequest request)
    {
        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            // 1. Handle Supplier (if it's a purchase)
            Guid? supplierId = request.SupplierId;
            if (request.IsPurchase && supplierId == null && !string.IsNullOrWhiteSpace(request.SupplierName))
            {
                var existingSupplier = await _uow.Suppliers.GetByNameAsync(request.SupplierName);
                if (existingSupplier == null)
                {
                    var newSupplier = new Supplier
                    {
                        Id = Guid.NewGuid(),
                        Name = request.SupplierName,
                        BranchId = request.BranchId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _uow.Suppliers.AddAsync(newSupplier);
                    supplierId = newSupplier.Id;
                }
                else
                {
                    supplierId = existingSupplier.Id;
                }
            }

            // 2. Prepare Purchase (if applicable)
            Purchase? purchase = null;
            if (request.IsPurchase)
            {
                purchase = new Purchase
                {
                    Id = Guid.NewGuid(),
                    PurchaseNumber = await GeneratePurchaseNumberAsync(),
                    SupplierId = supplierId,
                    BranchId = request.BranchId,
                    Notes = request.Notes,
                    PurchaseDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            // 3. System Settings for Margin/Tax
            var settings = await _uow.GetQueryable<SystemSetting>()
                .Where(s => s.BranchId == request.BranchId || s.BranchId == null)
                .ToListAsync();

            decimal marginPercentage = GetSettingValue(settings, "ExpectedProfitMargin", 30m) / 100m;
            bool isTaxEnabled = GetSettingValue(settings, "IsTaxEnabled", "true") == "true";
            decimal defaultTaxRate = GetSettingValue(settings, "DefaultTaxRate", 18m);

            decimal subTotal = 0;
            decimal taxTotal = 0;

            // 4. Process Items
            foreach (var itemDto in request.Items)
            {
                Product? product = null;

                if (itemDto.ProductId.HasValue && itemDto.ProductId.Value != Guid.Empty)
                {
                    product = await _uow.Products.GetByIdAsync(itemDto.ProductId.Value);
                }

                if (product == null)
                {
                    // Inline Product Creation
                    if (string.IsNullOrWhiteSpace(itemDto.ProductName))
                        throw new ArgumentException("ProductName is required for new products.");

                    product = new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = itemDto.ProductName,
                        CategoryId = itemDto.CategoryId,
                        SKU = itemDto.SKU,
                        SupplierId = supplierId,
                        BranchId = request.BranchId,
                        Stock = 0,
                        MinStock = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _uow.Products.AddAsync(product);
                }

                // Calculations
                decimal currentTaxRate = itemDto.TaxRate ?? (isTaxEnabled ? defaultTaxRate : 0);
                decimal lineSubTotal = itemDto.UnitCost * itemDto.Quantity;
                decimal lineTaxAmount = lineSubTotal * (currentTaxRate / 100m);
                decimal lineTotal = lineSubTotal + lineTaxAmount;

                // Inverted Acquisition Flow
                product.Cost = itemDto.UnitCost * (1 + (currentTaxRate / 100m));
                product.Price = product.Cost + (product.Cost * marginPercentage);
                product.TaxRate = currentTaxRate;
                product.UpdatedAt = DateTime.UtcNow;

                _uow.Products.Update(product);

                if (purchase != null)
                {
                    purchase.Items.Add(new PurchaseItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Quantity = (int)itemDto.Quantity,
                        UnitCost = itemDto.UnitCost,
                        TaxRate = currentTaxRate,
                        TaxAmount = lineTaxAmount,
                        LineTotal = lineTotal
                    });
                    
                    subTotal += lineSubTotal;
                    taxTotal += lineTaxAmount;
                }

                // Inventory Transaction (Kardex)
                await LogTransactionAsync(
                    product.Id,
                    itemDto.Quantity,
                    request.IsPurchase ? TransactionType.Purchase : TransactionType.Adjustment,
                    purchase?.PurchaseNumber ?? "ADJ-ENTRY",
                    purchaseId: purchase?.Id,
                    variantId: itemDto.VariantId);
            }

            if (purchase != null)
            {
                purchase.SubTotal = subTotal;
                purchase.TaxTotal = taxTotal;
                purchase.Total = subTotal + taxTotal;
                await _uow.Purchases.AddAsync(purchase);
            }

            // 5. Activity Log (Human Readable)
            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = request.BranchId,
                Message = request.IsPurchase 
                    ? $"Compra registrada: {purchase?.PurchaseNumber} de {request.SupplierName}"
                    : $"Ajuste de entrada registrado: {request.Notes}",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            };
            await _uow.ActivityLogs.AddAsync(activityLog);

            await _uow.CommitAsync();

            return purchase != null ? Map(purchase) : null;
        });
    }

    public async Task ProcessExitAsync(StockExitRequest request)
    {
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            foreach (var itemDto in request.Items)
            {
                if (!itemDto.ProductId.HasValue) throw new ArgumentException("ProductId is required for exits.");

                var product = await _uow.Products.GetByIdAsync(itemDto.ProductId.Value)
                    ?? throw new KeyNotFoundException($"Product {itemDto.ProductId} not found.");

                // Note: LogTransactionAsync handles Stock update for both Product and Variant
                await LogTransactionAsync(
                    product.Id,
                    itemDto.Quantity,
                    request.Type,
                    request.Notes ?? "EXIT-ENTRY",
                    variantId: itemDto.VariantId);
            }

            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = request.BranchId,
                Message = $"Salida de inventario registrada: {request.Notes}",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            };
            await _uow.ActivityLogs.AddAsync(activityLog);

            await _uow.CommitAsync();
        });
    }

    public async Task AdjustAsync(ManualStockAdjustmentDto dto)
    {
        if (dto.ProductId == Guid.Empty)
            throw new ArgumentException("ProductId is required");

        var product = await _uow.Products.GetByIdAsync(dto.ProductId)
            ?? throw new KeyNotFoundException($"Product {dto.ProductId} not found");

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            await LogTransactionAsync(
                dto.ProductId,
                dto.Quantity,
                dto.Type,
                dto.ReferenceNumber ?? $"ADJ-{DateTime.UtcNow:yyyyMMddHHmmss}",
                variantId: dto.VariantId);

            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = dto.BranchId ?? product.BranchId,
                Message = $"Ajuste manual de stock: {product.Name} ({(dto.Quantity > 0 ? "+" : "")}{dto.Quantity} unidades). Tipo: {dto.Type}",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            };
            await _uow.ActivityLogs.AddAsync(activityLog);

            await _uow.CommitAsync();
        });
    }

    public async Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync()
    {
        var purchases = await _uow.Purchases.GetAllAsync();
        return purchases.Select(Map);
    }

    public async Task<PurchaseDto?> GetPurchaseByIdAsync(Guid id)
    {
        var purchase = await _uow.Purchases.GetByIdAsync(id);
        return purchase != null ? Map(purchase) : null;
    }

    public async Task<IEnumerable<InventoryTransaction>> GetAllTransactionsAsync()
    {
        return await _uow.InventoryTransactions.GetAllAsync();
    }

    public async Task<IEnumerable<InventoryTransactionDto>> GetFilteredTransactionsAsync(
        Guid? branchId, Guid? productId, DateTime? startDate, DateTime? endDate, TransactionType? type, Guid? userId = null)
    {
        IQueryable<InventoryTransaction> query = _uow.GetQueryable<InventoryTransaction>()
            .Include(t => t.Product)
            .Include(t => t.Variant)
            .Include(t => t.Branch);

        if (branchId.HasValue)
            query = query.Where(t => t.BranchId == branchId.Value);
        
        if (productId.HasValue)
            query = query.Where(t => t.ProductId == productId.Value);
        
        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        
        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);
        
        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (!_tenantService.IsGlobalAdmin)
        {
            var myBranchId = _tenantService.BranchId;
            if (myBranchId.HasValue)
            {
                query = query.Where(t => t.BranchId == myBranchId.Value);
            }
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return transactions.Select(t => new InventoryTransactionDto(
            t.Id,
            t.ProductId,
            t.Product?.Name ?? "Unknown",
            t.VariantId,
            t.Variant != null ? $"{t.Variant.Color} {t.Variant.Size}".Trim() : null,
            (int)t.Type,
            t.Type.ToString(),
            t.Quantity,
            t.PreviousStock,
            t.NewStock,
            t.ReferenceNumber,
            t.TransactionDate,
            t.InvoiceId,
            t.PurchaseId,
            t.BranchId,
            t.Branch?.Name,
            t.UserId,
            t.User?.FullName
        ));
    }

    public async Task LogTransactionAsync(Guid productId, decimal quantity, TransactionType type, string? referenceNumber = null, Guid? invoiceId = null, Guid? purchaseId = null, Guid? variantId = null)
    {
        var product = await _uow.Products.GetByIdAsync(productId) 
            ?? throw new KeyNotFoundException("Product not found");

        decimal previousStock = product.Stock;
        decimal transactionQuantity = Math.Abs(quantity);
        
        // Determine if adding or subtracting
        bool isAddition = type == TransactionType.Purchase || (type == TransactionType.Adjustment && quantity > 0);

        if (variantId.HasValue)
        {
            var variant = await _uow.GetQueryable<ProductVariant>().FirstOrDefaultAsync(v => v.Id == variantId.Value)
                ?? throw new KeyNotFoundException("Variant not found");
                
            previousStock = variant.Stock;
            
            if (isAddition) variant.Stock += transactionQuantity;
            else variant.Stock -= transactionQuantity;
            
            _uow.Update(variant);
            
            // Sync parent product stock
            if (isAddition) product.Stock += transactionQuantity;
            else product.Stock -= transactionQuantity;
        }
        else
        {
            if (isAddition) product.Stock += transactionQuantity;
            else product.Stock -= transactionQuantity;
        }

        decimal newStock = product.Stock; // This is the new total stock
        if (variantId.HasValue)
        {
            // If it's a variant, let's log the variant's specific new stock in the transaction record
            var variant = await _uow.GetQueryable<ProductVariant>().AsNoTracking().FirstOrDefaultAsync(v => v.Id == variantId.Value);
            newStock = variant?.Stock ?? newStock;
        }

        product.UpdatedAt = DateTime.UtcNow;
        _uow.Products.Update(product);

        var transaction = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            VariantId = variantId,
            BranchId = product.BranchId,
            Type = type,
            Quantity = transactionQuantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            ReferenceNumber = referenceNumber,
            InvoiceId = invoiceId,
            PurchaseId = purchaseId,
            TransactionDate = DateTime.UtcNow,
            UserId = _tenantService.UserId
        };

        await _uow.InventoryTransactions.AddAsync(transaction);

        // Low Stock Check
        if (newStock <= product.MinStock && previousStock > product.MinStock)
        {
            await _notificationService.SendNotificationToBranchAsync(
                product.BranchId ?? Guid.Empty, 
                $"Stock bajo: {product.Name} (Queda: {newStock})", 
                "LowStock");
        }
    }

    public async Task<IEnumerable<InventoryTransaction>> GetKardexAsync(Guid productId)
    {
        return await _uow.InventoryTransactions.GetByProductIdAsync(productId);
    }

    private async Task<string> GeneratePurchaseNumberAsync()
    {
        var all = await _uow.Purchases.GetAllAsync();
        int next = all.Count() + 1;
        return $"COM-{next:D6}";
    }

    private decimal GetSettingValue(IEnumerable<SystemSetting> settings, string key, decimal defaultValue)
    {
        var setting = settings.FirstOrDefault(s => s.Key == key);
        return setting != null && decimal.TryParse(setting.Value, out var val) ? val : defaultValue;
    }

    private string GetSettingValue(IEnumerable<SystemSetting> settings, string key, string defaultValue)
    {
        var setting = settings.FirstOrDefault(s => s.Key == key);
        return setting?.Value ?? defaultValue;
    }

    private PurchaseDto Map(Purchase p) => new(
        p.Id,
        p.PurchaseNumber,
        p.SupplierId,
        p.Supplier?.Name ?? "Admin", // Fallback
        p.BranchId,
        p.Branch?.Name,
        p.PurchaseDate,
        p.SubTotal,
        p.TaxTotal,
        p.Total,
        p.Notes,
        p.Items.Select(item => new PurchaseItemDto(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? "Unknown",
            item.Quantity,
            item.UnitCost,
            item.TaxRate,
            item.TaxAmount,
            item.LineTotal
        )).ToList(),
        p.CreatedAt
    );
}
