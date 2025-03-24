namespace Shared.Application.Interfaces.Services;

public interface ISharedSaleService
{
    Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken);
}