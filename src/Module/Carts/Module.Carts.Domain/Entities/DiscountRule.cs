using Shared.Domain.Common;

namespace Module.Carts.Domain.Entities;

public class DiscountRule : BaseEntity
{
    public int MinQuantity { get; private set; }
    public int MaxQuantity { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public bool Active { get; private set; }

    private DiscountRule() { }

    public DiscountRule(int minQuantity, int maxQuantity, decimal discountPercentage, bool active = true)
    {
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        DiscountPercentage = discountPercentage;
        Active = active;
    }
}