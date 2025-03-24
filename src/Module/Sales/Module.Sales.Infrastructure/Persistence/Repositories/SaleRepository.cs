using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Sales.Infrastructure.Persistence.Repositories;

public class SaleRepository : ISaleRepository
{
    
    private readonly ISaleDbContext _context;

    public SaleRepository(ISaleDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Sale sale, CancellationToken cancellationToken)
    {
       await _context.Sales.AddAsync(sale, cancellationToken);
    }

    public void  Update(Sale sale)
    {
        var existing = _context.Sales.Local.FirstOrDefault(s => s.Id == sale.Id);
        if (existing is null)
        {
            _context.Sales.Attach(sale);
        }

        _context.Sales.Update(sale);
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByNumberAsync(long number, CancellationToken cancellationToken)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Number == number, cancellationToken);
    }

    public async Task<PaginatedList<Sale>> GetAllAsync(string? filter, string? orderBy, int page, int size, CancellationToken cancellationToken)
    {
        var gridifyQuery = new GridifyQuery
        {
            Filter = filter,
            OrderBy = orderBy,
            Page = page,
            PageSize = size
        };

        var pagingResult = await _context.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .GridifyAsync(gridifyQuery, cancellationToken);

        return new PaginatedList<Sale>(pagingResult.Data, pagingResult.Count, page, size);
    }
}