using Microsoft.Extensions.Logging;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Infrastructure.Persistence.Seeders;

public class CartDbContextInitializer
{
    private ILogger<CartDbContextInitializer> _logger;
    private readonly CartDbContext _context;
    
    public CartDbContextInitializer(CartDbContext context, ILogger<CartDbContextInitializer> logger)
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
            _logger.LogError(e, "An error occurred while seeding the cart database.");
            throw;
        }
    }
    
    public async Task TrySeedAsync()
    {
       await TrySeedDefaultRules();
    }
    
    public async Task TrySeedDefaultRules()
    {
        if (!_context.DiscountRules.Any())
        {
            var rules = new List<DiscountRule>
            {
                new(minQuantity: 4, maxQuantity: 9, discountPercentage: 10),
                new(minQuantity: 10, maxQuantity: 20, discountPercentage: 20)
            };

            await _context.DiscountRules.AddRangeAsync(rules);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded default discount rules.");
        }
    }
}