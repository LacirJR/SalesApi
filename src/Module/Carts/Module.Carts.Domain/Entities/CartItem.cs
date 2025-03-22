using Module.Carts.Domain.Exceptions;
using Shared.Domain.Common;

namespace Module.Carts.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    
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
            throw new CartDomainException("Quantity must be greater than zero.");

        Quantity = newQuantity;
    }

    public void ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new CartDomainException("Discount must be between 0% and 100%.");

        DiscountPercentage = discountPercentage;
    }

    public decimal GetTotalPrice()
    {
        var discountAmount = (UnitPrice * (decimal)(DiscountPercentage / 100)) * Quantity;
        return (UnitPrice * Quantity) - discountAmount;
    }
}