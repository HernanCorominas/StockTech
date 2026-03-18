using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(Guid id);
    Task<Client?> GetByDocumentAsync(string document);
    Task AddAsync(Client client);
    void Update(Client client);
    void Delete(Client client);
}
