using Module.Users.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Users.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Update(User user);
    Task<User?> RemoveByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PaginatedList<User>> GetAllAsync(string? filter, string? orderBy, int page, int size,
        CancellationToken cancellationToken);
}