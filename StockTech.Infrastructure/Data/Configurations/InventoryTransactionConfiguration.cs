using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ProductId)
            .IsRequired()
            .HasColumnName("product_id");

        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnName("type");

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(x => x.PreviousStock)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnName("previous_stock");

        builder.Property(x => x.NewStock)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnName("new_stock");

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(50)
            .HasColumnName("reference_number");

        builder.Property(x => x.TransactionDate)
            .IsRequired()
            .HasColumnName("transaction_date");

        builder.Property(x => x.InvoiceId)
            .HasColumnName("invoice_id");

        builder.Property(x => x.PurchaseId)
            .HasColumnName("purchase_id");

        // Relationships
        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
