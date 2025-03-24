using Shared.Domain.Common;

namespace Module.Sales.Domain.Entities;

public sealed class SaleItem :BaseEntity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal Total => Math.Round(UnitPrice * Quantity * (1 - DiscountPercentage / 100), 2,  MidpointRounding.AwayFromZero);
    
    public Sale Sale { get; private set; }
    
    private SaleItem() { }

    public SaleItem(Guid saleId, Guid productId, int quantity, decimal unitPrice, decimal discountPercentage)
    {
        Id = Guid.NewGuid();
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = discountPercentage;
    }
}