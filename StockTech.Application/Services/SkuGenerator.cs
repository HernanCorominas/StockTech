using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockTech.Domain.Interfaces;

namespace StockTech.Application.Services;

public class SkuGenerator : ISkuGenerator
{
    private readonly IUnitOfWork _uow;

    public SkuGenerator(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<string> GenerateSkuAsync(string? category)
    {
        string prefix = "SKU";
        string alphanumeric;
        bool isUnique = false;
        
        do
        {
            alphanumeric = GenerateRandomAlphanumeric(6);
            string candidate = $"{prefix}-{alphanumeric}";
            
            var exists = await _uow.Products.AsQueryable()
                .AnyAsync(p => p.SKU == candidate);
                
            if (!exists) 
            {
                isUnique = true;
                return candidate;
            }
        } while (!isUnique);

        return $"{prefix}-{alphanumeric}";
    }

    private string GenerateRandomAlphanumeric(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
