using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.SKU).HasColumnName("sku").HasMaxLength(50);
        builder.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Cost).HasColumnName("cost").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Stock).HasColumnName("stock");
        builder.Property(x => x.MinStock).HasColumnName("min_stock");
        builder.Property(x => x.CategoryId).HasColumnName("category_id");
        builder.HasOne(x => x.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(x => x.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Products)
               .HasForeignKey(x => x.BranchId);
               
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id");
        builder.HasOne(x => x.Supplier)
               .WithMany(s => s.Products)
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.SetNull);
        
        // Concurrency token
        builder.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        // Unique SKU Index
        builder.HasIndex(x => x.SKU).IsUnique();
    }
}
