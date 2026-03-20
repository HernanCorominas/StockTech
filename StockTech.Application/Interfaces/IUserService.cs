using StockTech.Domain.Entities;

namespace StockTech.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Role>> GetRolesAsync();
    Task ChangePasswordAsync(Guid userId, string newPassword);
}
