using StockTech.Application.DTOs.Auth;

namespace StockTech.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
