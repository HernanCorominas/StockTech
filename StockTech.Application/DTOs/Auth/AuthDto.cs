namespace StockTech.Application.DTOs.Auth;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string Token, string Username, string Role, DateTime ExpiresAt);
