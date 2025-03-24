using Shared.Application.Dtos;

namespace Shared.Application.Interfaces.Persistence;

public interface ISharedProductRepository
{
    Task<SharedProductDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);

}