using Module.Sales.Application.Interfaces.Persistence;
using Shared.Infrastructure.Persistence;

namespace Module.Sales.Infrastructure.Persistence;

public class SaleUnitOfWork : UnitOfWork<SaleDbContext>, ISaleUnitOfWork
{
    public SaleUnitOfWork(SaleDbContext context) : base(context) { }
}