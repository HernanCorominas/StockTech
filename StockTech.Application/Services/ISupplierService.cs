using StockTech.Application.DTOs.Suppliers;

namespace StockTech.Application.Services;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllAsync(Guid? branchId = null);
    Task<SupplierDto> GetByIdAsync(Guid id);
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
    Task<SupplierDto> UpdateAsync(Guid id, CreateSupplierDto dto);
    Task DeleteAsync(Guid id);
}
