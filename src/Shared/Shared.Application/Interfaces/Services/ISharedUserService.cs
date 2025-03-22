namespace Shared.Application.Interfaces.Services;

public interface ISharedUserService
{
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken);

}