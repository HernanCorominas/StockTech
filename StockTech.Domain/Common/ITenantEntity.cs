namespace StockTech.Domain.Common;

public interface ITenantEntity
{
    Guid? BranchId { get; set; }
}
