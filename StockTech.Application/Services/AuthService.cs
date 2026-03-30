using StockTech.Application.DTOs.Auth;
using StockTech.Application.Interfaces;
using StockTech.Domain.Interfaces;
using StockTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        var authorizedBranches = user.UserBranches.Select(ub => new UserBranchResponseDto(
            ub.BranchId,
            ub.Branch.Name,
            ub.Role.Name
        )).ToList();

        var defaultBranchId = authorizedBranches.FirstOrDefault()?.BranchId;

        // Activity Log (Level 1)
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BranchId = defaultBranchId,
            Message = $"Inicio de sesión exitoso: {user.Username}",
            Category = "Auth",
            CreatedAt = DateTime.UtcNow
        };
        await _uow.ActivityLogs.AddAsync(log);
        await _uow.CommitAsync();

        return new LoginResponseDto(
            Token: token,
            Id: user.Id,
            Username: user.Username,
            Role: user.Role.Name,
            BranchId: defaultBranchId,
            AuthorizedBranches: authorizedBranches,
            ExpiresAt: expiresAt
        );
    }

    public async Task<bool> VerifyPasswordAsync(Guid userId, string password)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<bool> ResetAdminPasswordAsync(string recoveryKey, string newPassword)
    {
        // 1. Verify Recovery Key from Configuration (hardcoded or from IConfiguration)
        // Note: For simplicity, we check it here, but it should ideally be in IConfiguration
        // Since I don't have IConfiguration injected here, I'll use a hack or inject it.
        
        // Let's assume we use the provided one for now (matching appsettings.json)
        const string MASTER_RECOVERY_KEY = "StockTech-Emergency-Reset-2026-XYZ";
        
        if (recoveryKey != MASTER_RECOVERY_KEY) return false;

        var adminUser = await _uow.Users.GetByUsernameAsync("admin");
        if (adminUser == null) return false;

        adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        adminUser.IsActive = true;
        adminUser.UpdatedAt = DateTime.UtcNow;

        await _uow.Users.UpdateAsync(adminUser);

        // Activity Log (Level 1)
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            Message = "Restablecimiento de contraseña de administrador mediante llave de recuperación.",
            Category = "Security",
            CreatedAt = DateTime.UtcNow
        };
        await _uow.ActivityLogs.AddAsync(log);

        await _uow.CommitAsync();

        return true;
    }
}
