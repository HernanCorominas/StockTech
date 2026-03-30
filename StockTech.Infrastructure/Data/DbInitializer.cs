using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;
using StockTech.Infrastructure.Data;
using StockTech.Domain.Constants;
using BCrypt.Net;

namespace StockTech.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(StockTechDbContext context)
    {
        // 1. Seed All Permissions from Constants
        var allPermissions = PermissionConstants.All.ToList();
        var existingPermissions = await context.Permissions.ToListAsync();
        
        foreach (var permName in allPermissions)
        {
            if (!existingPermissions.Any(p => p.Name == permName))
            {
                context.Permissions.Add(new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permName,
                    Description = $"Acceso a {permName}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        
        // Also ensure legacy "admin:*" exists for global access logic
        if (!existingPermissions.Any(p => p.Name == "admin:*") && !allPermissions.Contains("admin:*"))
        {
            context.Permissions.Add(new Permission
            {
                Id = Guid.NewGuid(),
                Name = "admin:*",
                Description = "Acceso total al sistema",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        
        await context.SaveChangesAsync();

        // 2. Ensure Roles exist
        var adminRoleId = Guid.Parse("4314c95d-1c22-4009-86a3-a7404fe7e6c2");
        var managerRoleId = Guid.Parse("b0e7a6d1-8f92-4c2a-9b3e-7a1d5e6c4b3a");
        var sellerRoleId = Guid.Parse("c1f8b7e2-9a03-5d3b-ac4f-8b2e6f7d5c4b");

        await EnsureRoleAsync(context, adminRoleId, "Admin", "Administrador Global del Sistema");
        await EnsureRoleAsync(context, managerRoleId, "Manager", "Gerente de Sucursal");
        await EnsureRoleAsync(context, sellerRoleId, "Seller", "Vendedor / Cajero");

        // 3. Map Permissions to Roles
        var permissions = await context.Permissions.ToListAsync();
        
        // Admin gets EVERYTHING
        var adminRole = await context.Roles.Include(r => r.Permissions).FirstAsync(r => r.Id == adminRoleId);
        foreach (var p in permissions)
        {
            if (!adminRole.Permissions.Any(ap => ap.Id == p.Id)) adminRole.Permissions.Add(p);
        }

        // Manager Permissions
        var managerRole = await context.Roles.Include(r => r.Permissions).FirstAsync(r => r.Id == managerRoleId);
        var managerPermNames = new[] { 
            PermissionConstants.InventoryRead, PermissionConstants.InventoryWrite,
            PermissionConstants.ProductCreate, PermissionConstants.ProductUpdate,
            PermissionConstants.SaleRead, PermissionConstants.SaleCreate,
            PermissionConstants.InvoiceRead, PermissionConstants.SupplierRead,
            PermissionConstants.SupplierCreate, PermissionConstants.SupplierUpdate,
            PermissionConstants.UserRead, PermissionConstants.BranchRead
        };
        foreach (var p in permissions.Where(p => managerPermNames.Contains(p.Name)))
        {
            if (!managerRole.Permissions.Any(mp => mp.Id == p.Id)) managerRole.Permissions.Add(p);
        }

        // Seller Permissions
        var sellerRole = await context.Roles.Include(r => r.Permissions).FirstAsync(r => r.Id == sellerRoleId);
        var sellerPermNames = new[] { 
            PermissionConstants.SaleCreate, PermissionConstants.SaleRead,
            PermissionConstants.InvoiceRead, PermissionConstants.ProductCreate, // Selles can sometimes create quick products? Maybe just read.
            PermissionConstants.InventoryRead
        };
        foreach (var p in permissions.Where(p => sellerPermNames.Contains(p.Name)))
        {
            if (!sellerRole.Permissions.Any(sp => sp.Id == p.Id)) sellerRole.Permissions.Add(p);
        }

        await context.SaveChangesAsync();

        // 4. Ensure Default Branch exists (Main Branch)
        var defaultBranchId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        var defaultBranch = await context.Branches.FirstOrDefaultAsync(b => b.Id == defaultBranchId);
        if (defaultBranch == null)
        {
            defaultBranch = new Branch
            {
                Id = defaultBranchId,
                Name = "Main Branch",
                Address = "Sistema Central",
                Phone = "000-000-0000",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Branches.Add(defaultBranch);
            await context.SaveChangesAsync();
        }

        // 5. Ensure admin user exists
        var adminUser = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Username.ToLower() == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "Administrator",
                Email = "admin@stocktech.com",
                RoleId = adminRoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            // Associate admin with the default branch
            var anyBranch = await context.Branches.FirstOrDefaultAsync();
            if (anyBranch != null)
            {
                context.UserBranches.Add(new UserBranch
                {
                    UserId = adminUser.Id,
                    BranchId = anyBranch.Id,
                    RoleId = adminRoleId
                });
                await context.SaveChangesAsync();
            }
        }

        // 6. Ensure System Settings exist
        if (!await context.SystemSettings.AnyAsync(s => s.Key == "ExpectedProfitMargin"))
        {
            context.SystemSettings.Add(new SystemSetting
            {
                Id = Guid.NewGuid(),
                Key = "ExpectedProfitMargin",
                Value = "0.30", // 30% default profit margin
                Description = "Margen de beneficio por defecto",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureRoleAsync(StockTechDbContext context, Guid id, string name, string description)
    {
        // Check by name first to avoid IX_roles_name violation
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == name || r.Id == id);
        if (role == null)
        {
            context.Roles.Add(new Role
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
}
