using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("product_variants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Size)
                .HasMaxLength(20);

            builder.Property(x => x.Color)
                .HasMaxLength(30);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SKU).IsUnique();
        }
    }
}
