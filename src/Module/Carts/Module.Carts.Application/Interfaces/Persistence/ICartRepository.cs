using Module.Carts.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Carts.Application.Interfaces.Persistence;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task<Cart?> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<PaginatedList<Cart>> GetAllAsync(string? filter, string? orderBy, int page, int size,
        CancellationToken cancellationToken);

    Task AddAsync(Cart cart, CancellationToken cancellationToken);
    void Update(Cart cart);
    Task RemoveByIdAsync(Guid id, CancellationToken cancellationToken);
}