using System;

namespace StockTech.Application.DTOs.Categories;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive = true
);

public record CreateCategoryDto(
    string Name,
    string? Description
);

public record UpdateCategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
);
