using FluentValidation;
using Module.Sales.Application.Commands.UpdateSaleCommand;

namespace Module.Sales.Application.Validations;

public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required.");

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch is required.")
            .MaximumLength(100).WithMessage("Branch cannot exceed 100 characters.");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");
    }
}