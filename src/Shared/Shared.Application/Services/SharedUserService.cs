using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Shared.Application.Services;

public class SharedUserService : ISharedUserService
{
    private readonly ISharedUserRepository  _sharedUserRepository;

    public SharedUserService(ISharedUserRepository sharedUserRepository)
    {
        _sharedUserRepository = sharedUserRepository;
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _sharedUserRepository.ExistsByIdAsync(userId, cancellationToken);
    }
}