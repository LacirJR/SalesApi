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
    
    [Fact]
    public async Task RemoveByIdAsync_Should_Remove_User_When_Exists()
    {
        var user = new User("remove@example.com", "removeUser", "ValidPassword123",
            new Name("Remove", "User"),
            new Address("City", "Street", 10, "12345678", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync(default);

        var removedUser = await _repository.RemoveByIdAsync(user.Id, CancellationToken.None);
        await _context.SaveChangesAsync(default);

        var userInDb = await _context.Users.FindAsync(user.Id);
        Assert.Null(userInDb);
        Assert.NotNull(removedUser);
        Assert.Equal(user.Email, removedUser.Email);
    }
    
    [Fact]
    public async Task RemoveByIdAsync_Should_Return_Null_When_User_Not_Found()
    {
        var removedUser = await _repository.RemoveByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(removedUser);
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Exists()
    {
        var user = new User("findbyid@example.com", "findUser", "ValidPassword123",
            new Name("Find", "User"),
            new Address("City", "Street", 10, "12345678", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync(default);

        var result = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Not_Found()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetAllAsync_Should_Return_Paginated_Users()
    {
        var users = new List<User>
        {
            new User("user1@example.com", "user1", "ValidPassword123",
                new Name("User1", "Test"), new Address("City1", "Street1", 1, "12345", new Geolocation("12.34", "56.78")),
                "999888777", UserRole.Admin, UserStatus.Active),

            new User("user2@example.com", "user2", "ValidPassword123",
                new Name("User2", "Test"), new Address("City2", "Street2", 2, "54321", new Geolocation("13.34", "57.78")),
                "888777666", UserRole.Customer, UserStatus.Active)
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync(default);

        var result = await _repository.GetAllAsync(null, "Username", 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Data.ToList().Count);
        Assert.Equal("user1@example.com", result.Data.ToList()[0].Email);
        Assert.Equal("user2@example.com", result.Data.ToList()[1].Email);
    }
    
    [Fact]
    public async Task GetAllAsync_Should_Return_EmptyList_When_No_Users()
    {
        var result = await _repository.GetAllAsync(null, "Username", 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }
}