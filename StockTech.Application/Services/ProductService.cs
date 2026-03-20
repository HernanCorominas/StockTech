using StockTech.Application.DTOs.Common;
using StockTech.Application.DTOs.Products;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow) => _uow = uow;

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

    public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        
        var result = await _uow.Products.GetPagedAsync(page, pageSize, search);
        return new PagedResult<ProductDto>
        {
            Items = result.Items.Select(Map),
            TotalCount = result.TotalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            SKU = dto.SKU?.Trim(),
            Price = dto.Price,
            Cost = dto.Cost,
            Stock = dto.Stock,
            MinStock = dto.MinStock,
            Category = dto.Category?.Trim()
        };

        await _uow.Products.AddAsync(product);
        await _uow.CommitAsync();
        return Map(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product is null) return null;

        product.Name = dto.Name.Trim();
        product.Description = dto.Description?.Trim();
        product.SKU = dto.SKU?.Trim();
        product.Price = dto.Price;
        product.Cost = dto.Cost;
        product.MinStock = dto.MinStock;
        product.Category = dto.Category?.Trim();
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        _uow.Products.Update(product);
        await _uow.CommitAsync();
        return Map(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product is null) return false;

        _uow.Products.Delete(product);
        await _uow.CommitAsync();
        return true;
    }

    private static ProductDto Map(Product p) => new(
        p.Id, p.Name, p.Description, p.SKU, p.Price, p.Cost,
        p.Stock, p.MinStock, p.Category, p.IsActive,
        p.Stock <= p.MinStock, p.CreatedAt
    );
}
