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
        if (rate < 0 || rate > 5) throw new ProductDomainException("A avaliação deve estar entre 0 e 5.");
        if (count < 0) throw new ProductDomainException("O número de avaliações não pode ser negativo.");
        
        Rate = rate;
        Count = count;
    }
}