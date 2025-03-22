using Microsoft.EntityFrameworkCore;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Infrastructure.Persistence.Repositories;

public class DiscountRuleRepository : IDiscountRuleRepository
{
    private ICartDbContext _context;

    public DiscountRuleRepository(ICartDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<DiscountRule>> GetActiveRulesAsync(CancellationToken cancellationToken)
    {
        return await _context.DiscountRules
            .Where(r => r.Active)
            .ToListAsync(cancellationToken);
    }
}