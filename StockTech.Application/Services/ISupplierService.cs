using StockTech.Application.DTOs.Suppliers;

namespace StockTech.Application.Services;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllAsync();
    Task<SupplierDto> GetByIdAsync(Guid id);
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
}
