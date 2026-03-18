using StockTech.Domain.Entities;

namespace StockTech.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
