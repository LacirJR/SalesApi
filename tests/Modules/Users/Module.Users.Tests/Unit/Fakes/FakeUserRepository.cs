using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common.Enums;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User("existing@email.com", "existingUser", "hashedPassword", 
            new Name("John", "Doe"), null, "123456789", UserRole.Customer, UserStatus.Active)
    };

    public void Update(User user)
    { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}