using FluentValidation;
using StockTech.Application.DTOs.Products;

namespace StockTech.Application.DTOs.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre es requerido.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
        RuleFor(x => x.Cost).GreaterThan(0).WithMessage("El costo debe ser mayor a cero.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
    }
}
