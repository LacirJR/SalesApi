using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using Module.Products.Infrastructure.Persistence;

namespace Module.Products.Tests.Unit.Fakes;

public class FakeProductsDbContext : DbContext, IProductDbContext
{
    public DbSet<Product> Products { get; set; } 
    public DbSet<Category> Categories { get; set; }  


    public FakeProductsDbContext(DbContextOptions<FakeProductsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}