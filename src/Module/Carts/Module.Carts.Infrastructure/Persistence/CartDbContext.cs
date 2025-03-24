using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Persistence;

namespace Module.Carts.Infrastructure.Persistence;

public class CartDbContext : ModuleDbContext, ICartDbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options, IMediator mediator, ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor) : base(options, mediator, currentUserService)
    {
    }

    protected override string Schema => "Carts";
    
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<DiscountRule> DiscountRules { get; set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}