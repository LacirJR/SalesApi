namespace Module.Sales.Application.Dtos;

public class SaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public double DiscountPercentage { get; set; }
    public decimal Total { get; set; }
}