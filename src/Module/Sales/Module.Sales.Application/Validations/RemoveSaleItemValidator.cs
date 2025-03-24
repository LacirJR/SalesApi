using FluentValidation;
using Module.Sales.Application.Commands.RemoveSaleItemCommand;

namespace Module.Sales.Application.Validations;

public class RemoveSaleItemValidator : AbstractValidator<RemoveSaleItemCommand>
{
    public RemoveSaleItemValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}