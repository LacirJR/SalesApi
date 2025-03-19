using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Module.Users.Infrastructure.Persistence.Repositories;
using Module.Users.Tests.Unit.Fakes;
using Shared.Domain.Common.Enums;
using Xunit;

namespace Module.Users.Tests.Unit.Repositories;

public class UserRepositoryTests
{
    private readonly IUserDbContext _context;
    private readonly IUserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FakeUserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FakeUserDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_User()
    {
        var user = new User("test@example.com", "testuser", "ValidPassword123",
            new Name("Test", "User"),
            new Address("City", "Street", 10, "12345678", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(default);

        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.NotNull(dbUser);
        Assert.Equal(user.Email, dbUser.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_User_When_Exists()
    {
        var user = new User("existing@example.com", "existingUser", "ValidPassword123",
            new Name("Existing", "User"),
            new Address("City", "Street", 10, "12345678", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        await _repository.AddAsync(user, CancellationToken.None);
        await _context.SaveChangesAsync(default);

        var result = await _repository.GetByEmailAsync("existing@example.com", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("existing@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var result = await _repository.GetByEmailAsync("notfound@example.com", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Should_Update_User_When_Exists()
    {
        var user = new User("existing@example.com", "existingUser", "ValidPassword123",
            new Name("Existing", "User"),
            new Address("City", "Street", 10, "12345678", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync(default);

        user.Update(user.Email, "newExistingUser", user.Password, new Name("Updated", "User"),
            new Address("NewCity", user.Address.Street, user.Address.Number, user.Address.Zipcode,
                new Geolocation(user.Address.Geolocation.Lat, user.Address.Geolocation.Long)), user.Phone, user.Role,
            user.Status);


        _repository.Update(user);
        await _context.SaveChangesAsync(default);

        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.NotNull(updatedUser);
        Assert.Equal("NewCity", updatedUser.Address.City);
        Assert.Equal("newExistingUser", updatedUser.Username);
        Assert.Equal("Updated", updatedUser.Name.Firstname);
    }
}