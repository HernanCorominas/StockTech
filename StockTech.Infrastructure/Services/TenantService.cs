using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _manualBranchId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            var claimValue = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(claimValue, out var userId)) return userId;
            return null;
        }
    }

    public bool IsGlobalAdmin
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return false;
            return user.IsInRole("Admin") == true || user.IsInRole("SystemAdmin") == true;
        }
    }

    public bool IsManager
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return false;
            return user.IsInRole("Manager") == true || user.IsInRole("BranchManager") == true;
        }
    }

    public IEnumerable<Guid> AuthorizedBranchIds
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return Enumerable.Empty<Guid>();

            var claimValue = user.FindFirstValue("AuthorizedBranches");
            if (string.IsNullOrEmpty(claimValue)) return Enumerable.Empty<Guid>();

            return claimValue.Split(',')
                .Select(s => Guid.TryParse(s, out var id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty);
        }
    }

    public Guid? BranchId
    {
        get
        {
            if (_manualBranchId != null) return _manualBranchId;

            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            var claimValue = context.User?.FindFirstValue("BranchId");
            
            if (Guid.TryParse(claimValue, out var branchId))
            {
                return branchId;
            }

            return null;
        }
    }

    public void SetBranchId(Guid? branchId)
    {
        _manualBranchId = branchId;
    }
}
