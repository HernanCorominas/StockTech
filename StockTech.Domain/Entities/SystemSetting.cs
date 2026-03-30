using StockTech.Domain.Common;

namespace StockTech.Domain.Entities;

public class SystemSetting : BaseEntity, ITenantEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? BranchId { get; set; }
}
