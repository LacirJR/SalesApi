using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Shared.Infrastructure.Common;

namespace Module.Products.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IProductDbContext _context;

    public ProductRepository(IProductDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        var existingUser = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);

        if (existingUser is null)
        {
            _context.Products.Attach(product);
        }

        _context.Products.Update(product);
    }

    public async Task<Product?> RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingProduct = await _context.Products.FindAsync(id, cancellationToken);

        if (existingProduct is null) return null;

        _context.Products.Remove(existingProduct);

        return existingProduct;
    }

    public async Task<bool> ExistsByTitleAndCategoryAsync(string title, string categoryName,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Name == categoryName, cancellationToken);

        if (category is null) return false;

        return await _context.Products.AnyAsync(p => p.Title == title && p.CategoryId == category.Id,
            cancellationToken);
    }

    public async Task<bool> AnyByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
    }
    
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PaginatedList<Product>> GetAllAsync(string? filter, string? orderBy, int page, int size,
        CancellationToken cancellationToken)
    {
        var gridifyQuery = new GridifyQuery()
        {
            Filter = filter,
            OrderBy = orderBy,
            Page = page,
            PageSize = size,
        };
        var pagingUsers = await _context.Products.Include(x => x.Category).AsNoTracking()
            .GridifyAsync(gridifyQuery, cancellationToken);

        return new PaginatedList<Product>(pagingUsers.Data, pagingUsers.Count, page, size);
    }
}