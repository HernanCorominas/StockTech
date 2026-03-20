using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("purchases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PurchaseNumber).HasColumnName("purchase_number").IsRequired().HasMaxLength(20);
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id");
        builder.Property(x => x.BranchId).HasColumnName("branch_id");
        builder.Property(x => x.PurchaseDate).HasColumnName("purchase_date");
        builder.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasColumnName("notes");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Supplier)
               .WithMany(s => s.Purchases)
               .HasForeignKey(x => x.SupplierId);

        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Purchases)
               .HasForeignKey(x => x.BranchId);
    }
}
