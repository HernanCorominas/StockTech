using StockTech.Application.DTOs.Auth;
using StockTech.Application.Interfaces;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork uow, IJwtService jwtService)
    {
        _uow = uow;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.Username);
        if (user == null || !user.IsActive) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new LoginResponseDto(
            Token: token,
            Username: user.Username,
            Role: user.Role,
            ExpiresAt: expiresAt
        );
    }
}
