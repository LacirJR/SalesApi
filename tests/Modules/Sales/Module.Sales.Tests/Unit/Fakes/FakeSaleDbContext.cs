using Microsoft.EntityFrameworkCore;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using Module.Sales.Infrastructure.Persistence;

namespace Module.Sales.Tests.Unit.Fakes;

public class FakeSaleDbContext : DbContext, ISaleDbContext
{
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    public FakeSaleDbContext(DbContextOptions<FakeSaleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaleDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}