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
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(100);
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        
        // Concurrency token
        builder.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();
    }
}
