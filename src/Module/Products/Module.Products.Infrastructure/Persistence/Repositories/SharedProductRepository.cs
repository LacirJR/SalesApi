using Module.Products.Application.Interfaces.Persistence;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;

namespace Module.Products.Infrastructure.Persistence.Repositories;

public class SharedProductRepository : ISharedProductRepository
{
    private readonly IProductDbContext  _context;

    public SharedProductRepository(IProductDbContext context)
    {
        _context = context;
    }

    public async Task<SharedProductDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { productId }, cancellationToken);
        if (product is null) return null;

        return new SharedProductDto()
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price
        };
    }
}