using Microsoft.Extensions.Logging;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common.Enums;

namespace Module.Users.Infrastructure.Persistence.Seeders;

public class UserDbContextInitializer
{
    private ILogger<UserDbContextInitializer> _logger;
    private readonly UserDbContext _context;

    public UserDbContextInitializer(UserDbContext context, ILogger<UserDbContextInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while seeding the user database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await TrySeedAddDefaultUserAsync();
    }

    public async Task TrySeedAddDefaultUserAsync()
    {
        var user = new User("test@email.com",
            "test",
            "admin123@",
            new Name("Test", "Admin"),
            new Address("test city", "street test", 1, "0000000", new Geolocation("90", "90")),
            "33333333333",
            UserRole.Admin,
            UserStatus.Active);

        user.Id = Guid.Parse("d6116266-fee9-4137-8923-9b9c8dba7859");

        var existingUser = await _context.Users.FindAsync(user.Id);

        if (existingUser is null)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}