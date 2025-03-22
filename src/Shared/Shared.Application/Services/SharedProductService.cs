using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Shared.Application.Services;

public class SharedProductService :  ISharedProductService
{
    private readonly ISharedProductRepository _productRepository;

    public SharedProductService(ISharedProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<SharedProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.GetByIdAsync(productId, cancellationToken);
    }
}