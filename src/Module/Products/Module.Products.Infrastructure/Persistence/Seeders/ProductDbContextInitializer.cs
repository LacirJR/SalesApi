using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;

namespace Module.Products.Infrastructure.Persistence.Seeders;

public class ProductDbContextInitializer
{
    private ILogger<ProductDbContextInitializer> _logger;
    private readonly ProductDbContext _context;
    
    public ProductDbContextInitializer(ProductDbContext context, ILogger<ProductDbContextInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while seeding the product database.");
            throw;
        }
    }
    
    public async Task TrySeedAsync()
    {
            await TrySeedCategoriesAsync();
            await TrySeedProductsAsync();
    }

    public async Task TrySeedCategoriesAsync()
    {
        if (!await _context.Categories.AnyAsync())
        {
            var uniqueCategoryNames = new HashSet<string>();

            var faker = new Faker("pt_BR");
            while (uniqueCategoryNames.Count < 20)
            {
                var category = faker.Commerce.Categories(1).First();
                uniqueCategoryNames.Add(category);
            }

            var categories = uniqueCategoryNames
                .Select(name => new Category(name))
                .ToList();

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task TrySeedProductsAsync()
    {
        if (!await _context.Products.AnyAsync())
        {
            var categories = await _context.Categories.ToListAsync();

            if (!categories.Any())
            {
                return;
            }

            var ratingFaker = new Faker<Rating>()
                .CustomInstantiator(f => new Rating(
                    f.Random.Decimal(0, 5),
                    f.Random.Int(0, 5000)));

            var productFaker = new Faker<Product>("pt_BR")
                .CustomInstantiator(f =>
                {
                    var categoryId = f.PickRandom(categories).Id;
                    var rating = ratingFaker.Generate();

                    return new Product(
                        f.Commerce.ProductName(),
                        f.Random.Decimal(10, 1000),
                        f.Lorem.Sentence(10),
                        categoryId,
                        f.Image.PicsumUrl(),
                        rating
                    );
                });

            var products = productFaker.Generate(1000);

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
    
  
}