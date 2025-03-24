using FluentValidation;
using Module.Sales.Application.Queries.GetSaleByNumberQuery;

namespace Module.Sales.Application.Validations;

public class GetSaleByNumberValidator : AbstractValidator<GetSaleByNumberQuery>
{
    public GetSaleByNumberValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Sale number must be greater than zero.");
    }
}
