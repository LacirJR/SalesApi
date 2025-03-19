using Microsoft.EntityFrameworkCore;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using Module.Users.Infrastructure.Persistence;

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
}