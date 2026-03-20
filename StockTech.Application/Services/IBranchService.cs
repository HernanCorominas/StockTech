using StockTech.Application.DTOs.Branches;

namespace StockTech.Application.Services;

public interface IBranchService
{
    Task<IEnumerable<BranchDto>> GetAllAsync();
    Task<BranchDto> GetByIdAsync(Guid id);
    Task<BranchDto> CreateAsync(CreateBranchDto dto);
}
