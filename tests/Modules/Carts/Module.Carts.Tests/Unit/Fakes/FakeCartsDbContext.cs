using Microsoft.EntityFrameworkCore;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Module.Carts.Infrastructure.Persistence;

namespace Module.Carts.Tests.Unit.Fakes;


public class FakeCartsDbContext : DbContext, ICartDbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<DiscountRule> DiscountRules { get; set; }

    public FakeCartsDbContext(DbContextOptions<FakeCartsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}