using FluentValidation;
using Module.Sales.Application.Queries.GetSaleByIdQuery;

namespace Module.Sales.Application.Validations;

public class GetSaleByIdValidator : AbstractValidator<GetSaleByIdQuery>
{
    public GetSaleByIdValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("SaleId is required.")
            .NotEqual(Guid.Empty).WithMessage("SaleId cannot be empty.");
    }
}