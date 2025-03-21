using Module.Products.Domain.Entities;

namespace Module.Products.Application.Interfaces.Persistence;

public interface ICategoryRepository
{
    Task<bool> ExistsByNameAsync(string categoryName, CancellationToken cancellationToken);
    Task<Category?> GetByNameAsync(string categoryName, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task<Category?> RemoveByNameAsync(string categoryName, CancellationToken cancellationToken);
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);

}