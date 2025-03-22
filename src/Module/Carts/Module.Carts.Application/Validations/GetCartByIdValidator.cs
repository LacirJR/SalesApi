using FluentValidation;
using Module.Carts.Application.Queries.GetCartByIdQuery;

namespace Module.Carts.Application.Validations;

public class GetCartByIdValidator : AbstractValidator<GetCartByIdQuery>
{
    public GetCartByIdValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required.")
            .NotEqual(Guid.Empty).WithMessage("CartId cannot be empty.");
    }
}