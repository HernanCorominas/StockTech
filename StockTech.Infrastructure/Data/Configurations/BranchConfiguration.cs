using StockTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockTech.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.Address).HasColumnName("address").IsRequired().HasMaxLength(500);
        builder.Property(x => x.Phone).HasColumnName("phone").IsRequired().HasMaxLength(20);
        builder.Property(x => x.ManagerName).HasColumnName("manager_name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Invoices)
               .WithOne(i => i.Branch)
               .HasForeignKey(i => i.BranchId);

        builder.HasMany(x => x.Purchases)
               .WithOne(p => p.Branch)
               .HasForeignKey(p => p.BranchId);
    }
}
