using StockTech.Application.DTOs.Suppliers;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _uow;

    public SupplierService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _uow.Suppliers.GetAllAsync();
        return suppliers.Select(Map);
    }

    public async Task<SupplierDto> GetByIdAsync(Guid id)
    {
        var supplier = await _uow.Suppliers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Supplier not found");
        return Map(supplier);
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            TaxId = dto.TaxId
        };

        await _uow.Suppliers.AddAsync(supplier);
        await _uow.CommitAsync();

        return Map(supplier);
    }

    private static SupplierDto Map(Supplier s) => new(
        s.Id,
        s.Name,
        s.ContactName,
        s.Phone,
        s.Email,
        s.TaxId,
        s.CreatedAt
    );
}
