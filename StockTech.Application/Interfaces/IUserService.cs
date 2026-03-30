using StockTech.Domain.Entities;

namespace StockTech.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user, string password, Guid? initialBranchId = null);
    Task UpdateAsync(User user, Guid? branchId = null);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Role>> GetRolesAsync();
    Task<Role> CreateRoleAsync(Role role, List<Guid> permissionIds);
    Task UpdateRoleAsync(Role role, List<Guid> permissionIds);
    Task DeleteRoleAsync(Guid id);
    Task<IEnumerable<Permission>> GetPermissionsAsync();
    Task ChangePasswordAsync(Guid userId, string newPassword);
}
