using StockTech.Application.DTOs.Clients;
using StockTech.Application.Interfaces;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _uow;

    public ClientService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ClientDto>> GetAllAsync()
    {
        var clients = await _uow.Clients.GetAllAsync();
        return clients.Select(Map);
    }

    public async Task<ClientDto?> GetByIdAsync(Guid id)
    {
        var client = await _uow.Clients.GetByIdAsync(id);
        return client is null ? null : Map(client);
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name.Trim(),
            Document = dto.Document.Trim(),
            ClientType = dto.ClientType,
            Email = dto.Email?.Trim(),
            Phone = dto.Phone?.Trim(),
            Address = dto.Address?.Trim()
        };

        await _uow.Clients.AddAsync(client);
        await _uow.CommitAsync();
        return Map(client);
    }

    private static ClientDto Map(Client c) => new(
        c.Id, c.Name, c.Document, c.ClientType,
        c.ClientType.ToString(), c.Email, c.Phone,
        c.Address, c.IsActive, c.CreatedAt
    );
}
