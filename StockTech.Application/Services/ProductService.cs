using StockTech.Application.DTOs.Common;
using StockTech.Application.DTOs.Products;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly ITenantService _tenantService;
    private readonly ISkuGenerator _skuGenerator;
    private readonly INotificationService _notificationService;

    public ProductService(IUnitOfWork uow, ITenantService tenantService, ISkuGenerator skuGenerator, INotificationService notificationService)
    {
        _uow = uow;
        _tenantService = tenantService;
        _skuGenerator = skuGenerator;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _uow.Products.GetAllAsync();
        return products.Select(Map);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        return product is null ? null : Map(product);
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(
        int page, int pageSize, string? search, string[]? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null,
        bool? lowStock = null, string? stockStatus = null,
        Guid? supplierId = null, Guid? branchId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var targetBranchId = branchId ?? (_tenantService.IsGlobalAdmin ? null : _tenantService.BranchId);
        
        var (items, totalCount) = await _uow.Products.GetPagedAsync(page, pageSize, search, categoryId, minPrice, maxPrice, lowStock, stockStatus, supplierId, targetBranchId);
        return new PagedResult<ProductDto>
        {
            Items = items.Select(Map),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            // Resolve and Validate BranchId
            var targetBranchId = dto.BranchId ?? _tenantService.BranchId;
            
            if (!targetBranchId.HasValue || targetBranchId == Guid.Empty)
            {
                throw new ArgumentException("El producto debe estar asociado a una sucursal.");
            }

            // Verify Branch existence
            var branch = await _uow.Branches.GetByIdAsync(targetBranchId.Value);
            if (branch == null)
            {
                throw new ArgumentException("La sucursal especificada no existe.");
            }

            // Auto-generate SKU
            string sku = await _skuGenerator.GenerateSkuAsync(dto.CategoryId?.ToString());

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                SKU = sku,
                Price = dto.Price,
                Cost = dto.Cost,
                Stock = dto.InitialStock ?? 0,
                MinStock = dto.MinStock,
                CategoryId = dto.CategoryId,
                BranchId = targetBranchId.Value,
                SupplierId = dto.SupplierId,
                Image = dto.Image
            };

            if (dto.Variants != null && dto.Variants.Any())
            {
                foreach (var v in dto.Variants)
                {
                    product.Variants.Add(new ProductVariant
                    {
                        Size = v.Size,
                        Color = v.Color,
                        SKU = v.SKU,
                        Price = v.Price,
                        Stock = v.Stock
                    });
                }
            }

            await _uow.Products.AddAsync(product);

            // Level 1 Audit
            await _uow.ActivityLogs.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = _tenantService.BranchId,
                Message = $"Producto creado: {product.Name} (SKU: {product.SKU})",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            });

            // Initial Stock Transaction
            if (dto.InitialStock.HasValue && dto.InitialStock.Value > 0)
            {
                await _uow.InventoryTransactions.AddAsync(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Type = StockTech.Domain.Enums.TransactionType.Adjustment,
                    Quantity = dto.InitialStock.Value,
                    PreviousStock = 0,
                    NewStock = dto.InitialStock.Value,
                    ReferenceNumber = "INICIAL",
                    TransactionDate = DateTime.UtcNow,
                    BranchId = targetBranchId.Value
                });
            }

            await _uow.CommitAsync();
            return Map(product);
        });
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product is null) return null;

            product.Name = dto.Name.Trim();
            product.Description = dto.Description?.Trim();
            // SKU is not editable as per requirements
            product.Price = dto.Price;
            product.Cost = dto.Cost;
            product.MinStock = dto.MinStock;
            product.CategoryId = dto.CategoryId;
            product.SupplierId = dto.SupplierId;
            product.Image = dto.Image;
            product.IsActive = dto.IsActive;
            
            if (dto.BranchId.HasValue && dto.BranchId != Guid.Empty && dto.BranchId != product.BranchId)
            {
                // Verify new Branch existence
                var branch = await _uow.Branches.GetByIdAsync(dto.BranchId.Value);
                if (branch == null) throw new ArgumentException("La sucursal especificada no existe.");
                product.BranchId = dto.BranchId.Value;
            }

            product.UpdatedAt = DateTime.UtcNow;

            // Sync Variants
            if (dto.Variants != null)
            {
                var existingVariantIds = dto.Variants.Where(v => v.Id.HasValue).Select(v => v.Id.Value).ToList();
                
                // Remove deleted variants
                var toRemove = product.Variants.Where(v => !existingVariantIds.Contains(v.Id)).ToList();
                foreach (var v in toRemove) { product.Variants.Remove(v); }

                foreach (var vDto in dto.Variants)
                {
                    if (vDto.Id.HasValue && vDto.Id != Guid.Empty)
                    {
                        var existing = product.Variants.FirstOrDefault(v => v.Id == vDto.Id);
                        if (existing != null)
                        {
                            existing.Size = vDto.Size;
                            existing.Color = vDto.Color;
                            existing.SKU = vDto.SKU;
                            existing.Price = vDto.Price;
                            existing.Stock = vDto.Stock;
                            existing.IsActive = vDto.IsActive;
                        }
                    }
                    else
                    {
                        product.Variants.Add(new ProductVariant
                        {
                            Size = vDto.Size,
                            Color = vDto.Color,
                            SKU = vDto.SKU,
                            Price = vDto.Price,
                            Stock = vDto.Stock,
                            IsActive = true
                        });
                    }
                }
            }

            _uow.Update(product);

            // Level 1 Audit
            await _uow.ActivityLogs.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = _tenantService.BranchId,
                Message = $"Producto actualizado: {product.Name}",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            });

            await _uow.CommitAsync();
            return Map(product);
        });
    }

    public async Task<bool> BatchUpdateStockAsync(List<BatchStockUpdateDto> dtos)
    {
        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            foreach(var dto in dtos)
            {
                var product = await _uow.Products.GetByIdAsync(dto.ProductId);
                if (product != null)
                {
                    product.Stock = dto.NewStock;
                    product.UpdatedAt = DateTime.UtcNow;
                    _uow.Update(product);
                }
            }
            await _uow.CommitAsync();

            // Re-check products in batch
            foreach(var dto in dtos)
            {
                var product = await _uow.Products.GetByIdAsync(dto.ProductId);
                if (product != null) await CheckLowStockAsync(product);
            }

            return true;
        });
    }

    private async Task CheckLowStockAsync(Product product)
    {
        if (product.Stock <= product.MinStock)
        {
            await _notificationService.SendNotificationToBranchAsync(
                product.BranchId.HasValue ? product.BranchId.Value : Guid.Empty,
                $"Alerta de Stock Bajo: {product.Name} (Restante: {product.Stock})",
                "LowStock"
            );
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _uow.ExecuteInTransactionAsync(async () =>
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product is null) return false;

            _uow.Remove(product);

            // Level 1 Audit
            await _uow.ActivityLogs.AddAsync(new ActivityLog
            {
                Id = Guid.NewGuid(),
                BranchId = _tenantService.BranchId,
                Message = $"Producto eliminado: {product.Name}",
                Category = "Inventory",
                CreatedAt = DateTime.UtcNow
            });

            await _uow.CommitAsync();
            return true;
        });
    }

    private static ProductDto Map(Product p) => new(
        p.Id, p.Name, p.Description, p.SKU, p.Price, p.Cost,
        (int)p.Stock, (int)p.MinStock, p.CategoryId, p.Category?.Name, p.IsActive,
        p.Stock <= p.MinStock, p.BranchId ?? Guid.Empty, p.SupplierId, p.Supplier?.Name, p.Image, p.CreatedAt,
        p.Variants.Select(v => new ProductVariantDto(
            v.Id, v.ProductId, v.Size, v.Color, v.SKU, v.Price, (int)v.Stock, v.IsActive
        )).ToList()
    );
}
