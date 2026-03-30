using Microsoft.AspNetCore.Authorization;

namespace StockTech.Infrastructure.Security;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}
