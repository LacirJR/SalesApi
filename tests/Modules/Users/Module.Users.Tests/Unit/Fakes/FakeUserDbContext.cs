using Bogus;
using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Module.Users.Infrastructure.Persistence;
using Shared.Domain.Common.Enums;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeUserDbContext : DbContext, IUserDbContext
{
    public DbSet<User> Users { get; set; }

    public FakeUserDbContext(DbContextOptions<FakeUserDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public void SeedData()
    {
        var faker = new Faker<User>()
            .CustomInstantiator(f => User.Create(
                f.Internet.Email(),
                f.Internet.UserName(),
                f.Internet.Password(),
                new Name(f.Name.FirstName(), f.Name.LastName()),
                new Address(
                    f.Address.City(),
                    f.Address.StreetName(),
                    f.Random.Int(1, 9999),
                    f.Address.ZipCode(),
                    new Geolocation(f.Address.Latitude().ToString(), f.Address.Longitude().ToString())
                ),
                f.Phone.PhoneNumber(),
                f.PickRandom<UserRole>(),
                f.PickRandom<UserStatus>()
            ));
        
        Users.AddRange(faker.Generate(10));
        SaveChanges();
    }
}