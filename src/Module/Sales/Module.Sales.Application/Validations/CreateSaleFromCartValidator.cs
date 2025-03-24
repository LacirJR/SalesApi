using FluentValidation;
using Module.Sales.Application.Commands.CreateFromCartCommand;
using Shared.Application.Interfaces.Services;

namespace Module.Sales.Application.Validations;

public class CreateSaleFromCartValidator : AbstractValidator<CreateSaleFromCartCommand>
{
    private readonly ISharedCartService _sharedCartService;

    public CreateSaleFromCartValidator(ISharedCartService sharedCartService)
    {
        _sharedCartService = sharedCartService;

        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("Cart ID is required.")
            .MustAsync(CartExists).WithMessage("Cart not found.");

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch is required.")
            .MaximumLength(100).WithMessage("Branch must not exceed 100 characters.");
    }

    private async Task<bool> CartExists(Guid cartId, CancellationToken cancellationToken)
    {
        return await _sharedCartService.GetCartByIdAsync(cartId, cancellationToken) is not null;
    }
}
