using Shared.Application.Dtos;

namespace Shared.Application.Interfaces.Services;

public interface ISharedProductService
{
    Task<SharedProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}