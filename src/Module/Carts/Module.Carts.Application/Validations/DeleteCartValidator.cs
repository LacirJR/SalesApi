using FluentValidation;
using Module.Carts.Application.Commands.DeleteCartCommand;

namespace Module.Carts.Application.Validations;

public class DeleteCartValidator: AbstractValidator<DeleteCartCommand>
{
    public DeleteCartValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required.")
            .NotEqual(Guid.Empty).WithMessage("CartId cannot be empty.");
    }
}
