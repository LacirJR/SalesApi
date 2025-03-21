using Shared.Domain.Common;

namespace Module.Carts.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public double DiscountPercentage { get; private set; }
    
    private CartItem() { }

    public CartItem(Guid cartId, Guid productId, int quantity, decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        Quantity = newQuantity;
    }

    public void ApplyDiscount(double discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new InvalidOperationException("Discount must be between 0% and 100%.");

        DiscountPercentage = discountPercentage;
    }

    public decimal GetTotalPrice()
    {
        var discountAmount = (UnitPrice * (decimal)(DiscountPercentage / 100)) * Quantity;
        return (UnitPrice * Quantity) - discountAmount;
    }
}