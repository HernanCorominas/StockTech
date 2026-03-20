namespace StockTech.Application.DTOs.Branches;

public record CreateBranchDto(
    string Name,
    string Address,
    string Phone,
    string ManagerName
);

public record BranchDto(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string ManagerName,
    DateTime CreatedAt
);
