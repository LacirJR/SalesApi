using FluentValidation;
using Module.Carts.Application.Dtos;
using Shared.Application.Interfaces.Services;

namespace Module.Carts.Application.Validations;

public class CartItemValidator : AbstractValidator<CartItemDto>
{
    private readonly ISharedProductService _sharedProductService;

    public CartItemValidator(ISharedProductService sharedProductService)
    {
        _sharedProductService = sharedProductService;

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .MustAsync(ProductExists).WithMessage("Product not found.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("You cannot add more than 20 units of the same product.");
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _sharedProductService.GetProductByIdAsync(productId, cancellationToken) is not null;
    }
}