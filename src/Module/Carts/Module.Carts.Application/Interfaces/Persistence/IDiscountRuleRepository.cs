using Module.Carts.Domain.Entities;

namespace Module.Carts.Application.Interfaces.Persistence;

public interface IDiscountRuleRepository
{
    Task<List<DiscountRule>> GetActiveRulesAsync(CancellationToken cancellationToken);
}