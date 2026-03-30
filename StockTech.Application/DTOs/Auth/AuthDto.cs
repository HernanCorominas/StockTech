namespace StockTech.Application.DTOs.Auth;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string Token, Guid Id, string Username, string Role, Guid? BranchId, IEnumerable<UserBranchResponseDto> AuthorizedBranches, DateTime ExpiresAt);

public record UserBranchResponseDto(Guid BranchId, string BranchName, string RoleName);
