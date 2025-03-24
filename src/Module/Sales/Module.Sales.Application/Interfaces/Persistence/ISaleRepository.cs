using Module.Sales.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Sales.Application.Interfaces.Persistence;

public interface ISaleRepository
{
    Task AddAsync(Sale sale, CancellationToken cancellationToken);
    void Update(Sale sale);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Sale?> GetByNumberAsync(long number, CancellationToken cancellationToken);
    Task<PaginatedList<Sale>> GetAllAsync(string? filter, string? orderBy, int page, int size, CancellationToken cancellationToken);
}
