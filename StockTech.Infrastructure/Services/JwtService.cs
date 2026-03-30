using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockTech.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config) => _config = config;

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("BranchId", user.UserBranches?.FirstOrDefault()?.BranchId.ToString() ?? string.Empty),
            new Claim("AuthorizedBranches", string.Join(",", user.UserBranches?.Select(ub => ub.BranchId.ToString()) ?? new List<string>())),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.Role.Permissions != null)
        {
            foreach (var permission in user.Role.Permissions)
            {
                claims.Add(new Claim("permission", permission.Name));
            }
        }

        // Ensure Admin/SystemAdmin always have admin:* permission
        if (user.Role.Name == "Admin" || user.Role.Name == "SystemAdmin")
        {
            if (user.Role.Permissions == null || !user.Role.Permissions.Any(p => p.Name == "admin:*"))
            {
                claims.Add(new Claim("permission", "admin:*"));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
