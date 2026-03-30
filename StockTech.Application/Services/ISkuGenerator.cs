namespace StockTech.Application.Services;

public interface ISkuGenerator
{
    Task<string> GenerateSkuAsync(string? category);
}
