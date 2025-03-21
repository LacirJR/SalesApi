using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Carts.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private ICartDbContext _context;

    public CartRepository(ICartDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
    }

    public async Task<Cart?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<PaginatedList<Cart>> GetAllAsync(string? filter, string? orderBy, int page, int size, CancellationToken cancellationToken)
    {
        var gridifyQuery = new GridifyQuery()
        {
            Filter = filter,
            OrderBy = orderBy,
            Page = page,
            PageSize = size
        };

        var pagingCarts = await _context.Carts
            .Include(c => c.Products)
            .AsNoTracking()
            .GridifyAsync(gridifyQuery, cancellationToken);

        return new PaginatedList<Cart>(pagingCarts.Data, pagingCarts.Count, page, size);
    }

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
    }

    public void Update(Cart cart)
    {
        var existingUser = _context.Carts.Local.FirstOrDefault(p => p.Id == cart.Id);

        if (existingUser is null)
        {
            _context.Carts.Attach(cart);
        }

        _context.Carts.Update(cart);
    }

    public async Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingCart = await _context.Carts.FindAsync(id, cancellationToken);
        
        if (existingCart is null) return;

        _context.Carts.Remove(existingCart);
        
    }
}