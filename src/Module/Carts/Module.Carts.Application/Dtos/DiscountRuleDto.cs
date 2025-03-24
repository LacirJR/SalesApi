namespace Module.Carts.Application.Dtos;

public class DiscountRuleDto
{
    public Guid Id { get; set; }
    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool Active { get; set; }
}