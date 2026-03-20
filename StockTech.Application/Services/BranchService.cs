using StockTech.Application.DTOs.Branches;
using StockTech.Domain.Entities;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _uow;

    public BranchService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        var branches = await _uow.Branches.GetAllAsync();
        return branches.Select(Map);
    }

    public async Task<BranchDto> GetByIdAsync(Guid id)
    {
        var branch = await _uow.Branches.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Branch not found");
        return Map(branch);
    }

    public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
    {
        var branch = new Branch
        {
            Name = dto.Name,
            Address = dto.Address,
            Phone = dto.Phone,
            ManagerName = dto.ManagerName
        };

        await _uow.Branches.AddAsync(branch);
        await _uow.CommitAsync();

        return Map(branch);
    }

    private static BranchDto Map(Branch b) => new(
        b.Id,
        b.Name,
        b.Address,
        b.Phone,
        b.ManagerName,
        b.CreatedAt
    );
}
