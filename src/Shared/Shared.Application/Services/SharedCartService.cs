using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Shared.Application.Services;

public class SharedCartService : ISharedCartService
{
    private readonly ISharedCartRepository _sharedCartRepository;

    public SharedCartService(ISharedCartRepository sharedCartRepository)
    {
        _sharedCartRepository = sharedCartRepository;
    }

    public async Task<SharedCartDto?> GetCartByIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        return await _sharedCartRepository.GetByIdAsync(cartId, cancellationToken);
    }

    public async Task FinalizeCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        await _sharedCartRepository.FinalizeCartAsync(cartId, cancellationToken);
    }

    public async Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        await _sharedCartRepository.RemoveProductByIdAsync(productId, cancellationToken);
    }
}