using StockTech.Application.DTOs.Clients;

namespace StockTech.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(Guid id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
}
