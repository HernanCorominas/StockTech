using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StockTech.Infrastructure.Data;

namespace StockTech.Infrastructure.Security;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly StockTechDbContext _context;

    public PermissionHandler(StockTechDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext authContext,
        PermissionRequirement requirement)
    {
        var userIdClaim = authContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return;

        if (!Guid.TryParse(userIdClaim.Value, out var userId)) return;

        // Fetch user with role and permissions
        var user = await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Role == null) return;

        // Check if user is global admin (special case for "admin:*")
        if (user.Role.Permissions.Any(p => p.Name == "admin:*"))
        {
            authContext.Succeed(requirement);
            return;
        }

        // Check for specific permission
        if (user.Role.Permissions.Any(p => p.Name == requirement.Permission))
        {
            authContext.Succeed(requirement);
        }
    }
}
