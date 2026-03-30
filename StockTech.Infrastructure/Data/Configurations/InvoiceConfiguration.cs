using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTech.Domain.Entities;

namespace StockTech.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.InvoiceNumber).HasColumnName("invoice_number").IsRequired().HasMaxLength(20);
        builder.Property(x => x.ClientId).HasColumnName("client_id");
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.InvoiceDate).HasColumnName("invoice_date");
        builder.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(18,2)");
        builder.Property(x => x.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(5,2)");
        builder.Property(x => x.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.Notes).HasColumnName("notes");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.HasOne(x => x.Client).WithMany(c => c.Invoices).HasForeignKey(x => x.ClientId);
        
        builder.HasOne(x => x.Branch)
               .WithMany(b => b.Invoices)
               .HasForeignKey(x => x.BranchId);
    }
}
