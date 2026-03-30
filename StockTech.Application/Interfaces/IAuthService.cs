using StockTech.Application.DTOs.Auth;

namespace StockTech.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<bool> VerifyPasswordAsync(Guid userId, string password);
    Task<bool> ResetAdminPasswordAsync(string recoveryKey, string newPassword);
}
