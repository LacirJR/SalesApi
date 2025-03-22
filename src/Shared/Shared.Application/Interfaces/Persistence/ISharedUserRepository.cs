namespace Shared.Application.Interfaces.Persistence;

public interface ISharedUserRepository
{
    Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken);

}