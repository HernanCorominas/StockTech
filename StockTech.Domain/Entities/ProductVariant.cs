namespace StockTech.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? Size { get; set; }
    public string? Color { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int MinStock { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
