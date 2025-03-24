namespace Shared.Application.Interfaces.Persistence;

public interface ISharedSaleRepository
{
    Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}