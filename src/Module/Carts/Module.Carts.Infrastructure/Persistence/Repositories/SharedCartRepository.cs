using Microsoft.EntityFrameworkCore;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Enums;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;

namespace Module.Carts.Infrastructure.Persistence.Repositories;

public class SharedCartRepository : ISharedCartRepository
{
    private readonly ICartDbContext _context;

    public SharedCartRepository(ICartDbContext context)
    {
        _context = context;
    }

    public async Task<SharedCartDto?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == cartId && x.Status == CartStatus.Active, cancellationToken);

        if (cart is null)
            return null;

        return new SharedCartDto()
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Date = cart.Date,
            IsFinalized = cart.IsFinalized,
            Products = cart.Products.Select(x => new SharedCartItemDto()
            {
                ProductId = x.ProductId,
                DiscountPercentage = x.DiscountPercentage,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList()
        };
    }

    public async Task FinalizeCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts.FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);
        if (cart is null || cart.IsFinalized) return;

        cart.FinalizeCart();
        _context.Carts.Update(cart);
    }

    public async Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var cartItems = await _context.CartItems.Include(x => x.Cart)
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Cart.Status == CartStatus.Active)
            {
                cartItem.Cart.RemoveItem(productId);

                if (cartItem.Cart.Products.Count == 0)
                {
                    cartItem.Cart.FinalizeCart();
                }
            }
            
        }
    }
}