using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StockTech.Application.Interfaces;

namespace StockTech.API.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            if (context.Request.Headers.TryGetValue("X-Branch-Id", out var branchIdHeader))
            {
                if (Guid.TryParse(branchIdHeader, out var branchId))
                {
                    // Global Admins can switch to ANY branch
                    if (tenantService.IsGlobalAdmin)
                    {
                        tenantService.SetBranchId(branchId);
                    }
                    // Managers can switch ONLY to authorized branches
                    else if (tenantService.IsManager)
                    {
                        if (tenantService.AuthorizedBranchIds.Contains(branchId))
                        {
                            tenantService.SetBranchId(branchId);
                        }
                        else
                        {
                            // If trying to access unauthorized branch, we can either ignore or return 403.
                            // For simplicity and consistent filtering, we'll ignore (keep default JWT branch) 
                            // but ideally we should log this attempt.
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}
