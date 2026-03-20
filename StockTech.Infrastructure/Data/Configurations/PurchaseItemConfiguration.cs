using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("purchase_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PurchaseId).HasColumnName("purchase_id");
        builder.Property(x => x.ProductId).HasColumnName("product_id");
        builder.Property(x => x.Quantity).HasColumnName("quantity");
        builder.Property(x => x.UnitCost).HasColumnName("unit_cost").HasColumnType("decimal(18,2)");
        builder.Property(x => x.LineTotal).HasColumnName("line_total").HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.HasOne(x => x.Purchase).WithMany(p => p.Items).HasForeignKey(x => x.PurchaseId);
        builder.HasOne(x => x.Product).WithMany(p => p.PurchaseItems).HasForeignKey(x => x.ProductId);
    }
}
