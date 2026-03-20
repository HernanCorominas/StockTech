using StockTech.Application.DTOs.Products;

using StockTech.Application.DTOs.Common;

namespace StockTech.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize, string? search);
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}
