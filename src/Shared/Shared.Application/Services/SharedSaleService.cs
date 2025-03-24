using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Shared.Application.Services;

public class SharedSaleService : ISharedSaleService
{
    private readonly ISharedSaleRepository _sharedSaleRepository;

    public SharedSaleService(ISharedSaleRepository sharedSaleRepository)
    {
        _sharedSaleRepository = sharedSaleRepository;
    }

    public async Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        await _sharedSaleRepository.RemoveProductByIdAsync(productId, cancellationToken);
    }
}