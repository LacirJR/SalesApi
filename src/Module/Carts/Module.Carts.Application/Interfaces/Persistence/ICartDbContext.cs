using Microsoft.EntityFrameworkCore;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Application.Interfaces.Persistence;

public interface ICartDbContext
{
    public DbSet<Cart> Carts { get; }
    public DbSet<CartItem> CartItems { get; }
    public DbSet<DiscountRule> DiscountRules { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}