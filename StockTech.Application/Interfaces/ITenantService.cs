namespace StockTech.Application.Interfaces;

public interface ITenantService
{
    Guid? UserId { get; }
    Guid? BranchId { get; }
    bool IsGlobalAdmin { get; }
    bool IsManager { get; }
    IEnumerable<Guid> AuthorizedBranchIds { get; }
    void SetBranchId(Guid? branchId);
}
