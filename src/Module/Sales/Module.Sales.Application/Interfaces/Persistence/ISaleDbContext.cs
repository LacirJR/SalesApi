using Microsoft.EntityFrameworkCore;
using Module.Sales.Domain.Entities;

namespace Module.Sales.Application.Interfaces.Persistence;

public interface ISaleDbContext
{
    public DbSet<Sale> Sales { get; }
    public DbSet<SaleItem> SaleItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}