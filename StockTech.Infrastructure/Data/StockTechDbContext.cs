using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data;

public class StockTechDbContext : DbContext
{
    public StockTechDbContext(DbContextOptions<StockTechDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
            e.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
            e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(200);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.Username).IsUnique();
        });

        // Clients
        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("clients");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            e.Property(x => x.Document).HasColumnName("document").IsRequired().HasMaxLength(20);
            e.Property(x => x.ClientType).HasColumnName("client_type");
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
            e.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // Products
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.SKU).HasColumnName("sku").HasMaxLength(50);
            e.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
            e.Property(x => x.Cost).HasColumnName("cost").HasColumnType("decimal(18,2)");
            e.Property(x => x.Stock).HasColumnName("stock");
            e.Property(x => x.MinStock).HasColumnName("min_stock");
            e.Property(x => x.Category).HasColumnName("category").HasMaxLength(100);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // Invoices
        modelBuilder.Entity<Invoice>(e =>
        {
            e.ToTable("invoices");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InvoiceNumber).HasColumnName("invoice_number").IsRequired().HasMaxLength(20);
            e.Property(x => x.ClientId).HasColumnName("client_id");
            e.Property(x => x.InvoiceDate).HasColumnName("invoice_date");
            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(18,2)");
            e.Property(x => x.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(5,2)");
            e.Property(x => x.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(18,2)");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(18,2)");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Client).WithMany(c => c.Invoices).HasForeignKey(x => x.ClientId);
        });

        // InvoiceItems
        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.ToTable("invoice_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InvoiceId).HasColumnName("invoice_id");
            e.Property(x => x.ProductId).HasColumnName("product_id");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(18,2)");
            e.Property(x => x.LineTotal).HasColumnName("line_total").HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Invoice).WithMany(i => i.Items).HasForeignKey(x => x.InvoiceId);
            e.HasOne(x => x.Product).WithMany(p => p.InvoiceItems).HasForeignKey(x => x.ProductId);
        });

        // Purchases
        modelBuilder.Entity<Purchase>(e =>
        {
            e.ToTable("purchases");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.PurchaseNumber).HasColumnName("purchase_number").IsRequired().HasMaxLength(20);
            e.Property(x => x.Supplier).HasColumnName("supplier").IsRequired().HasMaxLength(200);
            e.Property(x => x.PurchaseDate).HasColumnName("purchase_date");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(18,2)");
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // PurchaseItems
        modelBuilder.Entity<PurchaseItem>(e =>
        {
            e.ToTable("purchase_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.PurchaseId).HasColumnName("purchase_id");
            e.Property(x => x.ProductId).HasColumnName("product_id");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitCost).HasColumnName("unit_cost").HasColumnType("decimal(18,2)");
            e.Property(x => x.LineTotal).HasColumnName("line_total").HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Purchase).WithMany(p => p.Items).HasForeignKey(x => x.PurchaseId);
            e.HasOne(x => x.Product).WithMany(p => p.PurchaseItems).HasForeignKey(x => x.ProductId);
        });
    }
}
