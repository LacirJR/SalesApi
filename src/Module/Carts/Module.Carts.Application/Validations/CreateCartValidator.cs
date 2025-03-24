using FluentValidation;
using Module.Carts.Application.Commands.CreateCartCommand;
using Shared.Application.Interfaces.Services;

namespace Module.Carts.Application.Validations;

public class CreateCartValidator : AbstractValidator<CreateCartCommand>
{
    private readonly ISharedUserService _sharedUserService;
    private readonly ISharedProductService _sharedProductService;

    public CreateCartValidator(ISharedUserService sharedUserService, ISharedProductService sharedProductService)
    {
        _sharedUserService = sharedUserService;
        _sharedProductService = sharedProductService;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(UserExists).WithMessage("User not found.");

        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("At least one product is required.");

        RuleForEach(x => x.Products)
            .SetValidator(new CartItemValidator(sharedProductService));
    }

    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _sharedUserService.UserExistsAsync(userId, cancellationToken);
    }
}