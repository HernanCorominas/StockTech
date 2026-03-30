namespace StockTech.Domain.Constants;

public static class PermissionConstants
{
    public const string InventoryRead = "inventory:read";
    public const string InventoryWrite = "inventory:write";
    public const string ProductCreate = "product:create";
    public const string ProductUpdate = "product:update";
    public const string ProductDelete = "product:delete";
    public const string StockAdjust = "stock:adjust";
    
    public const string SaleCreate = "sale:create";
    public const string SaleRead = "sale:read";
    public const string InvoiceRead = "invoice:read";
    
    public const string UserRead = "user:read";
    public const string UserCreate = "user:create";
    public const string UserUpdate = "user:update";
    public const string UserDelete = "user:delete";
    
    public const string BranchRead = "branch:read";
    public const string BranchUpdate = "branch:update";
    
    public const string AuditRead = "audit:read";
    
    public const string SupplierRead = "supplier:read";
    public const string SupplierCreate = "supplier:create";
    public const string SupplierUpdate = "supplier:update";
    public const string SupplierDelete = "supplier:delete";
    
    public const string SettingsRead = "settings:read";
    public const string SettingsUpdate = "settings:update";
    
    public static IEnumerable<string> All => new[]
    {
        InventoryRead, InventoryWrite, ProductCreate, ProductUpdate, ProductDelete, StockAdjust,
        SaleCreate, SaleRead, InvoiceRead,
        UserRead, UserCreate, UserUpdate, UserDelete,
        BranchRead, BranchUpdate,
        AuditRead,
        SupplierRead, SupplierCreate, SupplierUpdate, SupplierDelete,
        SettingsRead, SettingsUpdate
    };
}
