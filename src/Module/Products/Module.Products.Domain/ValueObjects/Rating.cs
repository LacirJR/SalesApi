using Microsoft.EntityFrameworkCore;
using Module.Products.Domain.Exceptions;

namespace Module.Products.Domain.ValueObjects;

[Owned]
public class Rating
{
    public decimal Rate { get; private set; }
    public int Count { get; private set; }

    public Rating(decimal rate, int count)
    {
        if (rate < 0 || rate > 5) throw new ProductDomainException("The rating must be between 0 and 5.");
        if (count < 0) throw new ProductDomainException("The number of ratings cannot be negative.");
        
        Rate = rate;
        Count = count;
    }
}