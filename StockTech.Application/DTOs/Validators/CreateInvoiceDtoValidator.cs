using FluentValidation;
using StockTech.Application.DTOs.Invoices;

namespace StockTech.Application.DTOs.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty().WithMessage("El cliente es requerido.");
        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 1)
            .WithMessage("El tax rate debe estar entre 0 y 1 (ej: 0.18 para 18%).");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("La factura debe tener al menos un item.")
            .Must(items => items != null && items.Any()).WithMessage("La factura debe tener al menos un item.");

        RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemDtoValidator());
    }
}

public class CreateInvoiceItemDtoValidator : AbstractValidator<CreateInvoiceItemDto>
{
    public CreateInvoiceItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("El producto es requerido.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
    }
}
