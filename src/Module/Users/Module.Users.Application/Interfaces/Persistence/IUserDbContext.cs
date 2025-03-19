using Microsoft.EntityFrameworkCore;
using Module.Users.Domain.Entities;

namespace Module.Users.Application.Interfaces.Persistence;

public interface IUserDbContext
{
    public DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}