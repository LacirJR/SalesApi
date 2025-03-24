using FluentValidation;
using Module.Sales.Application.Commands.FinishSaleCommand;

namespace Module.Sales.Application.Validations;

public class FinishSaleValidator : AbstractValidator<FinishSaleCommand>
{
    public FinishSaleValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required.");
    }
}