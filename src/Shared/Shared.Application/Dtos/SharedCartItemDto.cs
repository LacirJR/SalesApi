namespace Shared.Application.Dtos;

public class SharedCartItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice => (UnitPrice * Quantity) - ((UnitPrice * (decimal)(DiscountPercentage / 100)) * Quantity);
}