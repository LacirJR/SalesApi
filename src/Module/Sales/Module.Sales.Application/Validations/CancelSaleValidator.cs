using FluentValidation;
using Module.Sales.Application.Commands.CancelSaleCommand;

namespace Module.Sales.Application.Validations;

public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required.");
    }
}