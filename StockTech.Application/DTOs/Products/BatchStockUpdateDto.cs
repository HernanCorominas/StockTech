namespace StockTech.Application.DTOs.Products;

public record BatchStockUpdateDto(Guid ProductId, int NewStock);
