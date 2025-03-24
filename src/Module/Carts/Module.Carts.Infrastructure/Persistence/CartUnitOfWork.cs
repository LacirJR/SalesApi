using Module.Carts.Application.Interfaces.Persistence;
using Shared.Infrastructure.Persistence;

namespace Module.Carts.Infrastructure.Persistence;

public class CartUnitOfWork : UnitOfWork<CartDbContext>, ICartUnitOfWork
{
    public CartUnitOfWork(CartDbContext context) : base(context) { }
}