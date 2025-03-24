using Shared.Application.Dtos;

namespace Shared.Application.Interfaces.Persistence;

public interface ISharedCartRepository
{
    Task<SharedCartDto?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task FinalizeCartAsync(Guid cartId, CancellationToken cancellationToken);
    Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}