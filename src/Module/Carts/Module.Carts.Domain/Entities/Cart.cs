using Shared.Domain.Common;

namespace Module.Carts.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    private readonly List<CartItem> _products = new();
    public IReadOnlyCollection<CartItem> Products => _products.AsReadOnly();

    private Cart() { }

    public Cart(Guid userId, DateTime date)
    {
        UserId = userId;
        Date = date;
    }

    public void AddItem(CartItem item)
    {
        var existingItem = _products.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existingItem != null)
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

    public void ClearCart()
    {
        _products.Clear();
    }
}