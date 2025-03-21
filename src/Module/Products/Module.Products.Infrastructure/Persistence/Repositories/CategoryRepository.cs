using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;

namespace Module.Products.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IProductDbContext _context;

    public CategoryRepository(IProductDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(string categoryName, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Name == categoryName, cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string categoryName, CancellationToken cancellationToken)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c != null && c.Name == categoryName,
            cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(category, cancellationToken);

    }

    public async Task<Category?> RemoveByNameAsync(string categoryName, CancellationToken cancellationToken)
    {
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Name == categoryName, cancellationToken);
    
        if (existingCategory is null) return null;

        _context.Categories.Remove(existingCategory);
        
        return existingCategory;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories.ToListAsync(cancellationToken);
    }
}