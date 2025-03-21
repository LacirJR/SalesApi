using Microsoft.EntityFrameworkCore;
using Module.Products.Domain.Entities;

namespace Module.Products.Application.Interfaces.Persistence;

public interface IProductDbContext
{
    public DbSet<Product> Products { get; }
    public DbSet<Category> Categories { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}