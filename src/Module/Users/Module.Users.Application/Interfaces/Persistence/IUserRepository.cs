using Module.Users.Domain.Entities;

namespace Module.Users.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Update(User user);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}