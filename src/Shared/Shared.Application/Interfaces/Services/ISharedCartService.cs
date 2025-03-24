using Shared.Application.Dtos;

namespace Shared.Application.Interfaces.Services;

public interface ISharedCartService
{
    Task<SharedCartDto?> GetCartByIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task FinalizeCartAsync(Guid cartId, CancellationToken cancellationToken);
    Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}