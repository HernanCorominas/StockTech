using StockTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockTech.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.ContactName).HasColumnName("contact_name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasColumnName("phone").IsRequired().HasMaxLength(20);
        builder.Property(x => x.Email).HasColumnName("email").IsRequired().HasMaxLength(200);
        builder.Property(x => x.TaxId).HasColumnName("tax_id").IsRequired().HasMaxLength(20);
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Branch)
               .WithMany()
               .HasForeignKey(x => x.BranchId);

        builder.HasMany(x => x.Purchases)
               .WithOne(p => p.Supplier)
               .HasForeignKey(p => p.SupplierId);
    }
}
