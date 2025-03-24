using Module.Products.Application.Interfaces.Persistence;
using Shared.Infrastructure.Persistence;

namespace Module.Products.Infrastructure.Persistence;

public class ProductUnitOfWork : UnitOfWork<ProductDbContext>, IProductUnitOfWork
{
    public ProductUnitOfWork(ProductDbContext context) : base(context) { }
}