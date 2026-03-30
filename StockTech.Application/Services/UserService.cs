using Microsoft.EntityFrameworkCore;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;

    public UserService(IUnitOfWork unitOfWork, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    public async Task<User> CreateAsync(User user, string password, Guid? initialBranchId = null)
    {
        var role = (await _unitOfWork.GetSetAsync<Role>()).FirstOrDefault(r => r.Id == user.RoleId);
        bool isAdmin = role?.Name == "Admin" || role?.Name == "SystemAdmin";

        if (role?.Name == "Admin" && !_tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("Solo administradores globales pueden crear otros administradores.");
        }

        // Mandatory branch for non-admins
        var targetBranchId = initialBranchId ?? _tenantService.BranchId;
        if (!isAdmin && (!targetBranchId.HasValue || targetBranchId == Guid.Empty))
        {
            throw new InvalidOperationException("Cada usuario que no sea administrador debe estar asociado a una sucursal.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;

        await _unitOfWork.Users.AddAsync(user);

        // Association Logic
        if (targetBranchId.HasValue)
        {
            var userBranch = new UserBranch
            {
                UserId = user.Id,
                BranchId = targetBranchId.Value,
                RoleId = user.RoleId
            };
            await _unitOfWork.AddAsync(userBranch);
        }

        await _unitOfWork.CommitAsync();
        return user;
    }

    public async Task UpdateAsync(User user, Guid? branchId = null)
    {
        var existingUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
        if (existingUser == null) return;

        // Protection against role escalation or modifying other Admins
        var newRole = (await _unitOfWork.GetSetAsync<Role>()).FirstOrDefault(r => r.Id == user.RoleId);
        bool isAdmin = newRole?.Name == "Admin" || newRole?.Name == "SystemAdmin";

        if (newRole?.Name == "Admin" && !_tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("Solo administradores globales pueden asignar el rol de administrador.");
        }

        // Prevent modifying other admins if they are both admins
        if (existingUser.Role.Name == "Admin" && user.Id != _tenantService.UserId && _tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("No tienes permisos para modificar a otro administrador.");
        }

        // Branch Association Logic for Updates
        var targetBranchId = branchId ?? _tenantService.BranchId;
        
        if (!isAdmin && (!targetBranchId.HasValue || targetBranchId == Guid.Empty))
        {
            // If it's a non-admin, they MUST have a branch.
            throw new InvalidOperationException("Cada usuario que no sea administrador debe estar asociado a una sucursal.");
        }

        if (targetBranchId.HasValue && targetBranchId != Guid.Empty)
        {
            // Update or Insert relationship
            var queryable = _unitOfWork.GetQueryable<UserBranch>();
            var existingUB = await queryable.FirstOrDefaultAsync(ub => ub.UserId == user.Id);
            
            if (existingUB != null)
            {
                // Remove the actual composite key entry and add the new one or just update if we have a way
                // Since PK is {UserId, BranchId}, we must remove and re-add if branch changed.
                if (existingUB.BranchId != targetBranchId.Value)
                {
                    _unitOfWork.Remove(existingUB);
                    await _unitOfWork.AddAsync(new UserBranch 
                    { 
                        UserId = user.Id, 
                        BranchId = targetBranchId.Value,
                        RoleId = user.RoleId
                    });
                }
                else
                {
                    existingUB.RoleId = user.RoleId;
                    _unitOfWork.Update(existingUB);
                }
            }
            else
            {
                await _unitOfWork.AddAsync(new UserBranch 
                { 
                    UserId = user.Id, 
                    BranchId = targetBranchId.Value,
                    RoleId = user.RoleId 
                });
            }
        }

        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.RoleId = user.RoleId;
        existingUser.IsActive = user.IsActive;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(existingUser);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return;

        if (user.Role.Name == "Admin" && id != _tenantService.UserId && _tenantService.IsGlobalAdmin)
        {
            throw new UnauthorizedAccessException("No tienes permisos para eliminar a otro administrador.");
        }

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        return await _unitOfWork.GetQueryable<Role>()
            .Include(r => r.Permissions)
            .ToListAsync();
    }

    public async Task<Role> CreateRoleAsync(Role role, List<Guid> permissionIds)
    {
        role.CreatedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;
        
        if (!role.BranchId.HasValue || role.BranchId == Guid.Empty)
        {
            role.BranchId = _tenantService.BranchId;
        }
        
        var permissions = await _unitOfWork.GetQueryable<Permission>()
            .Where(p => permissionIds.Contains(p.Id))
            .ToListAsync();
            
        role.Permissions = permissions;
        
        await _unitOfWork.AddAsync(role);
        await _unitOfWork.CommitAsync();
        return role;
    }

    public async Task UpdateRoleAsync(Role role, List<Guid> permissionIds)
    {
        var existingRole = await _unitOfWork.GetQueryable<Role>()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == role.Id);
            
        if (existingRole == null) return;
        
        existingRole.Name = role.Name;
        existingRole.Description = role.Description;
        existingRole.UpdatedAt = DateTime.UtcNow;
        
        var permissions = await _unitOfWork.GetQueryable<Permission>()
            .Where(p => permissionIds.Contains(p.Id))
            .ToListAsync();
            
        existingRole.Permissions = permissions;
        
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteRoleAsync(Guid id)
    {
        var role = await _unitOfWork.GetQueryable<Role>().FirstOrDefaultAsync(r => r.Id == id);
        if (role != null)
        {
            if (role.Name == "Admin") throw new InvalidOperationException("No se puede eliminar el rol de Administrador.");
            
            _unitOfWork.Remove(role);
            await _unitOfWork.CommitAsync();
        }
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync()
    {
        return await _unitOfWork.GetQueryable<Permission>().ToListAsync();
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CommitAsync();
    }
}
