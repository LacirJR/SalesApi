using Module.Products.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Products.Application.Interfaces.Persistence;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Update(Product product);
    Task<Product?> RemoveByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByTitleAndCategoryAsync(string title, string categoryName, CancellationToken cancellationToken);
    Task<bool> AnyByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedList<Product>> GetAllAsync(
        string? filter, string? orderBy, int page, int size, CancellationToken cancellationToken);

}