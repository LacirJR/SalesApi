using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common.Enums;
using Shared.Infrastructure.Common;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User("existing@email.com", "existingUser", "hashedPassword", 
            new Name("John", "Doe"), null, "123456789", UserRole.Customer, UserStatus.Active)
    };

    public void Update(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
            _users.Add(user);
        }
    }

    public Task<User?> RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user is not null)
        {
            _users.Remove(user);
        }

        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    }

    public Task<PaginatedList<User>> GetAllAsync(string? filter, string? orderBy, int page, int size, CancellationToken cancellationToken)
    {
        return Task.FromResult(new PaginatedList<User>(_users, _users.Count, page, size));
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}