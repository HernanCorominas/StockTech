using StockTech.Domain.Enums;

namespace StockTech.Application.DTOs.Clients;

public record CreateClientDto(
    string Name,
    string Document,
    ClientType ClientType,
    string? Email,
    string? Phone,
    string? Address
);

public record ClientDto(
    Guid Id,
    string Name,
    string Document,
    ClientType ClientType,
    string ClientTypeName,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime CreatedAt
);
