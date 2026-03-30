using StockTech.Application.DTOs.Suppliers;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _uow;
    private readonly ITenantService _tenantService;

    public SupplierService(IUnitOfWork uow, ITenantService tenantService)
    {
        _uow = uow;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync(Guid? branchId = null)
    {
        var targetBranchId = branchId ?? _tenantService.BranchId;
        var suppliers = await _uow.Suppliers.GetAllAsync();
        
        if (targetBranchId.HasValue && targetBranchId != Guid.Empty)
        {
            suppliers = suppliers.Where(s => s.BranchId == null || s.BranchId == targetBranchId.Value);
        }

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
        ValidateEmail(dto.Email);

        var targetBranchId = dto.BranchId ?? _tenantService.BranchId;
        if (!targetBranchId.HasValue || targetBranchId == Guid.Empty)
        {
            throw new ArgumentException("El proveedor debe estar asociado a una sucursal.");
        }

        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            TaxId = dto.TaxId,
            BranchId = targetBranchId.Value
        };

        await _uow.Suppliers.AddAsync(supplier);
        await _uow.CommitAsync();

        return Map(supplier);
    }

    public async Task<SupplierDto> UpdateAsync(Guid id, CreateSupplierDto dto)
    {
        var supplier = await _uow.Suppliers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Supplier not found");

        ValidateEmail(dto.Email);

        var targetBranchId = dto.BranchId ?? _tenantService.BranchId;
        if (!targetBranchId.HasValue || targetBranchId == Guid.Empty)
        {
            throw new ArgumentException("El proveedor debe estar asociado a una sucursal.");
        }

        supplier.Name = dto.Name;
        supplier.ContactName = dto.ContactName;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.TaxId = dto.TaxId;
        supplier.BranchId = targetBranchId.Value;

        await _uow.Suppliers.UpdateAsync(supplier);
        await _uow.CommitAsync();

        return Map(supplier);
    }

    public async Task DeleteAsync(Guid id)
    {
        var supplier = await _uow.Suppliers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Supplier not found");

        await _uow.Suppliers.DeleteAsync(supplier);
        await _uow.CommitAsync();
    }

    private void ValidateEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email) && !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new InvalidOperationException("El formato del correo electrónico no es válido.");
        }
    }

    private static SupplierDto Map(Supplier s) => new(
        s.Id,
        s.Name,
        s.ContactName,
        s.Phone,
        s.Email,
        s.TaxId,
        s.BranchId,
        s.Branch?.Name,
        s.CreatedAt
    );
}
