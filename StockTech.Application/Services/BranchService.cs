using Microsoft.EntityFrameworkCore;
using StockTech.Application.DTOs.Branches;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _uow;
    private readonly ITenantService _tenantService;

    public BranchService(IUnitOfWork uow, ITenantService tenantService)
    {
        _uow = uow;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        if (_tenantService.IsGlobalAdmin)
        {
            return (await _uow.Branches.GetAllAsync()).Select(Map);
        }

        if (_tenantService.BranchId.HasValue)
        {
            var branch = await _uow.Branches.GetByIdAsync(_tenantService.BranchId.Value);
            return branch != null ? new[] { Map(branch) } : Enumerable.Empty<BranchDto>();
        }

        return Enumerable.Empty<BranchDto>();
    }

    public async Task<BranchDto> GetByIdAsync(Guid id)
    {
        if (!_tenantService.IsGlobalAdmin && _tenantService.BranchId != id)
        {
            throw new UnauthorizedAccessException("No tiene permisos para ver esta sucursal.");
        }

        var branch = await _uow.Branches.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Branch not found");
        return Map(branch);
    }

    public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
    {
        var existing = await _uow.Branches.GetByNameAsync(dto.Name);
        if (existing != null)
        {
            throw new InvalidOperationException($"Ya existe una sucursal con el nombre '{dto.Name}'.");
        }

        var branch = new Branch
        {
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            ManagerId = dto.ManagerId,
            IsActive = dto.IsActive
        };

        await _uow.Branches.AddAsync(branch);
        
        // Level 1 Audit
        await _uow.ActivityLogs.AddAsync(new ActivityLog
        {
            Id = Guid.NewGuid(),
            Message = $"Sucursal creada: {branch.Name}",
            Category = "System",
            CreatedAt = DateTime.UtcNow
        });

        await _uow.CommitAsync();

        // Sync manager's sucursal
        if (branch.ManagerId.HasValue)
        {
            var managerRole = (await _uow.GetSetAsync<Role>()).FirstOrDefault(r => r.Name == "Manager");
            if (managerRole != null)
            {
                await _uow.AddAsync(new UserBranch
                {
                    UserId = branch.ManagerId.Value,
                    BranchId = branch.Id,
                    RoleId = managerRole.Id
                });
                await _uow.CommitAsync();
            }
        }

        var created = await _uow.Branches.GetByIdAsync(branch.Id);
        return Map(created!);
    }

    public async Task<BranchDto> UpdateAsync(Guid id, UpdateBranchDto dto)
    {
        if (!_tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("Solo administradores globales pueden actualizar sucursales.");
        }

        var existingWithName = await _uow.Branches.GetByNameAsync(dto.Name);
        if (existingWithName != null && existingWithName.Id != id)
        {
            throw new InvalidOperationException($"Ya existe otra sucursal con el nombre '{dto.Name}'.");
        }

        var branch = await _uow.Branches.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Branch not found");

        var oldManagerId = branch.ManagerId;
        branch.Name = dto.Name;
        branch.Address = dto.Address;
        branch.Phone = dto.Phone;
        branch.ManagerId = dto.ManagerId;
        branch.IsActive = dto.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;

        _uow.Branches.Update(branch);
        
        // Level 1 Audit
        await _uow.ActivityLogs.AddAsync(new ActivityLog
        {
            Id = Guid.NewGuid(),
            Message = $"Sucursal actualizada: {branch.Name}",
            Category = "System",
            CreatedAt = DateTime.UtcNow
        });

        await _uow.CommitAsync();

        // Sync manager's sucursal if it changed
        if (branch.ManagerId.HasValue && branch.ManagerId != oldManagerId)
        {
            var managerRole = (await _uow.GetSetAsync<Role>()).FirstOrDefault(r => r.Name == "Manager");
            if (managerRole != null)
            {
                var existingUB = await _uow.GetQueryable<UserBranch>()
                    .FirstOrDefaultAsync(ub => ub.UserId == branch.ManagerId.Value && ub.BranchId == branch.Id);

                if (existingUB == null)
                {
                    await _uow.AddAsync(new UserBranch
                    {
                        UserId = branch.ManagerId.Value,
                        BranchId = branch.Id,
                        RoleId = managerRole.Id
                    });
                }
                else
                {
                    existingUB.RoleId = managerRole.Id;
                }
                await _uow.CommitAsync();
            }
        }

        return Map(branch);
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!_tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("Solo administradores globales pueden eliminar sucursales.");
        }

        var branch = await _uow.Branches.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Branch not found");

        _uow.Branches.Delete(branch);
        
        // Level 1 Audit
        await _uow.ActivityLogs.AddAsync(new ActivityLog
        {
            Id = Guid.NewGuid(),
            Message = $"Sucursal eliminada: {branch.Name}",
            Category = "System",
            CreatedAt = DateTime.UtcNow
        });

        await _uow.CommitAsync();
    }

    private static BranchDto Map(Branch b) => new(
        b.Id,
        b.Name,
        b.Address,
        b.Phone,
        b.ManagerId,
        b.Manager?.FullName,
        b.IsActive,
        b.CreatedAt
    );
}
