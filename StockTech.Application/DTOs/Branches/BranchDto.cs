namespace StockTech.Application.DTOs.Branches;

public record CreateBranchDto(
    string Name,
    string Address,
    string Phone,
    Guid? ManagerId,
    bool IsActive
);

public record BranchDto(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    Guid? ManagerId,
    string? ManagerName,
    bool IsActive,
    DateTime CreatedAt
);

public record UpdateBranchDto(
    string Name,
    string Address,
    string Phone,
    Guid? ManagerId,
    bool IsActive
);
