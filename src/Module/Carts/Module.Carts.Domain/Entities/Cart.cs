using System.ComponentModel.DataAnnotations.Schema;
using Module.Carts.Domain.Enums;
using Module.Carts.Domain.Exceptions;
using Shared.Domain.Common;

namespace Module.Carts.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    private readonly List<CartItem> _products = new();
    public IReadOnlyCollection<CartItem> Products => _products.AsReadOnly();
    public CartStatus Status { get; private set; } = CartStatus.Active;
    
    [NotMapped]
    public bool IsFinalized => Status == CartStatus.Finalized;
    private Cart()
    {
    }

    public Cart(Guid userId, DateTime date)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Date = date;
    }

    public void AddItem(CartItem item)
    {
        var existingItem = _products.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
        else
        {
            _products.Add(item);
        }
    }

    public void RemoveItem(Guid productId)
    {
        var item = _products.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _products.Remove(item);
        }
    }

    public void ApplyRuleDiscount(List<DiscountRule> rules)
    {
        foreach (var product in _products)
        {
            var rule = rules.Where(x => product.Quantity >= x.MinQuantity && product.Quantity <= x.MaxQuantity)
                .OrderByDescending(x => x.MinQuantity)
                .FirstOrDefault();
            
            if(rule is not null)
                product.ApplyDiscount(rule.DiscountPercentage);
            else
             product.ApplyDiscount(0);
            
        }
    }

    public void UpdateItems(List<CartItem> updatedItems)
    {
        var updatedItemsDict = updatedItems.ToDictionary(i => i.ProductId, i => i);

        foreach (var existingItem in _products.ToList())
        {
            if (updatedItemsDict.TryGetValue(existingItem.ProductId, out var newItem))
            {
                var newQuantity = newItem.Quantity;

                if (newQuantity > 20)
                    throw new CartDomainException(
                        $"Cannot add more than 20 units of product {existingItem.ProductId}.");

                existingItem.UpdateQuantity(newQuantity);
                updatedItemsDict.Remove(existingItem.ProductId);
            }
            else
            {
                _products.Remove(existingItem);
            }
        }

        foreach (var newItem in updatedItemsDict.Values)
        {
            if (newItem.Quantity > 20)
                throw new CartDomainException($"Cannot add more than 20 units of product {newItem.ProductId}.");

            _products.Add(newItem);
        }
    }
    
    public void FinalizeCart()
    {
        Status = CartStatus.Finalized;
    }
}