using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Common;
using StockTech.Domain.Entities;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Data;

public class StockTechDbContext : DbContext
{
    private readonly ITenantService _tenantService;
    private bool _isSaving = false;

    public StockTechDbContext(
        DbContextOptions<StockTechDbContext> options, 
        ITenantService tenantService) : base(options) 
    { 
        _tenantService = tenantService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockTechDbContext).Assembly);

        // UserBranch Composite Key & Relationships
        modelBuilder.Entity<UserBranch>()
            .HasKey(ub => new { ub.UserId, ub.BranchId });

        modelBuilder.Entity<UserBranch>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserBranches)
            .HasForeignKey(ub => ub.UserId);

        modelBuilder.Entity<UserBranch>()
            .HasOne(ub => ub.Branch)
            .WithMany(b => b.UserBranches)
            .HasForeignKey(ub => ub.BranchId);

        modelBuilder.Entity<UserBranch>()
            .HasOne(ub => ub.Role)
            .WithMany(r => r.UserBranches)
            .HasForeignKey(ub => ub.RoleId);

        // Category - Product Relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = Guid.Parse("fb8d4fae-7dec-11d0-a765-00a0c91e6bf6"), Name = "Belleza", Description = "Productos de cuidado personal y belleza" },
            new Category { Id = Guid.Parse("fa2f082d-72a2-b281-0081-8b9cad0e1f20"), Name = "Tecnología", Description = "Equipos electrónicos y accesorios" },
            new Category { Id = Guid.Parse("f3b8d1b6-0b3b-4b1a-9c1a-1a2b3c4d5e6f"), Name = "Accesorios", Description = "Complementos y accesorios varios" },
            new Category { Id = Guid.Parse("fb4c9e2c-71c4-5c2b-ac2b-2b3c4d5e6f7a"), Name = "Ropa", Description = "Prendas de vestir y calzado" }
        );

        // Apply Global Query Filter for Multi-tenancy
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(StockTechDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);
                method?.Invoke(this, new object[] { modelBuilder });
            }
        }
    }


    private void ApplyTenantFilter<T>(ModelBuilder modelBuilder) where T : class, ITenantEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => (_tenantService.IsGlobalAdmin && !_tenantService.BranchId.HasValue) || e.BranchId == _tenantService.BranchId || e.BranchId == null);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_isSaving) return await base.SaveChangesAsync(cancellationToken);
        _isSaving = true;

        try
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }
        catch (Exception ex)
        {
            var errorPath = @"c:\Users\braul\OneDrive\Escritorio\StockTech\backend\error_debug.txt";
            var errorMessage = $"[{DateTime.Now}] Error in SaveChangesAsync:\n{ex.Message}\nInner: {ex.InnerException?.Message}\nStack: {ex.StackTrace}\n\n";
            System.IO.File.AppendAllText(errorPath, errorMessage);
            throw;
        }
        finally
        {
            _isSaving = false;
        }
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    if (_tenantService.BranchId.HasValue && _tenantService.BranchId != Guid.Empty)
                    {
                        entry.Entity.BranchId = _tenantService.BranchId;
                    }
                    
                    if (entry.State == EntityState.Added && (entry.Entity.BranchId == null || entry.Entity.BranchId == Guid.Empty))
                    {
                        // Check if this is a system action (no user yet, likely login/auth phase)
                        // or a Global Admin (SystemAdmin/Admin roles)
                        bool isSystemOrGlobalAdmin = _tenantService.IsGlobalAdmin || _tenantService.UserId == null;

                        if (!isSystemOrGlobalAdmin || entry.Entity is Product)
                        {
                            if (entry.Entity.BranchId == null || entry.Entity.BranchId == Guid.Empty)
                            {
                                throw new InvalidOperationException($"BranchId is required for entity {entry.Entity.GetType().Name}.");
                            }
                        }
                        
                        // For Global Admins (other entities), we allow a null BranchId (Global Entity/Log)
                        // as per user request: "le pide branch a admin cuando admin es el global. No deberia pedirle branch."
                    }
                    break;
            }
        }

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry);
            auditEntry.EntityName = entry.Entity.GetType().Name;
            auditEntry.UserId = _tenantService.UserId?.ToString();
            auditEntry.BranchId = _tenantService.BranchId;
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = "Create";
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = "Delete";
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.AuditType = "Update"; // Explicitly set here
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        return auditEntries;
    }

    private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return;

        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAuditLog());
        }

        await base.SaveChangesAsync();
    }
}

internal class AuditEntry
{
    public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        Entry = entry;
    }

    public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
    public string? UserId { get; set; }
    public string EntityName { get; set; } = null!;
    public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
    public List<string> ChangedColumns { get; } = new List<string>();
    public string AuditType { get; set; } = "Update"; // Default to Update
    public Guid? BranchId { get; set; }

    public AuditLog ToAuditLog()
    {
        var audit = new AuditLog();
        audit.UserId = UserId;
        audit.Action = AuditType ?? "Update";
        audit.TableName = EntityName;
        audit.CreatedAt = DateTime.UtcNow;
        audit.BranchId = BranchId;
        audit.KeyValues = JsonSerializer.Serialize(KeyValues);
        audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
        audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
        return audit;
    }
}
