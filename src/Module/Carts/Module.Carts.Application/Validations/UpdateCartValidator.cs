using FluentValidation;
using Module.Carts.Application.Commands.UpdateCartCommand;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Module.Carts.Application.Validations;

public class UpdateCartValidator : AbstractValidator<UpdateCartCommand>
{
    private readonly ISharedUserService _sharedUserService;
    public UpdateCartValidator( ISharedProductService sharedProductService, ISharedUserService sharedUserService)
    {
        _sharedUserService = sharedUserService;
        
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required.")
            .NotEqual(Guid.Empty).WithMessage("CartId cannot be empty.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(UserExists).WithMessage("User not found.");

        RuleForEach(x => x.Products)
            .SetValidator(new CartItemValidator(sharedProductService));
    }
    
    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _sharedUserService.UserExistsAsync(userId, cancellationToken);
    }
    
}