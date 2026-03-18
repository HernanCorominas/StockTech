using StockTech.Application.DTOs.Purchases;

namespace StockTech.Application.Interfaces;

public interface IPurchaseService
{
    Task<IEnumerable<PurchaseDto>> GetAllAsync();
    Task<PurchaseDto?> GetByIdAsync(Guid id);
    Task<PurchaseDto> CreateAsync(CreatePurchaseDto dto);
}
