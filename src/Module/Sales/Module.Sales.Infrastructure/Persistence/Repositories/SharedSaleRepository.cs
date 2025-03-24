using Microsoft.EntityFrameworkCore;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Enums;
using Shared.Application.Interfaces.Persistence;

namespace Module.Sales.Infrastructure.Persistence.Repositories;

public class SharedSaleRepository : ISharedSaleRepository
{
    private readonly ISaleDbContext _context;

    public SharedSaleRepository(ISaleDbContext context)
    {
        _context = context;
    }

    public async Task RemoveProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var salesItems = await _context.SaleItems.Include(x => x.Sale)
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);

        foreach (var saleItem in salesItems)
        {
            if (saleItem.Sale.Status == SaleStatus.Active)
            {
                saleItem.Sale.CancelItem(productId);
            }
        }
        
    }
}