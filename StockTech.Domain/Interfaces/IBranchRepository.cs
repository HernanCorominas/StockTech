using StockTech.Domain.Entities;

namespace StockTech.Domain.Interfaces;

public interface IBranchRepository
{
    Task<IEnumerable<Branch>> GetAllAsync();
    Task<Branch?> GetByIdAsync(Guid id);
    Task AddAsync(Branch branch);
    void Update(Branch branch);
    void Delete(Branch branch);
}
