using Module.Products.Domain.ValueObjects;
using Shared.Domain.Common;

namespace Module.Products.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string Title { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public string Image { get; private set; }
    public Rating Rating { get; private set; }
    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; }
    private Product() { }

    public Product(string title, decimal price, string description, Guid categoryId, string image, Rating rating)
    {
        Id = Guid.NewGuid();
        Title = title;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        Image = image;
        Rating = rating;
    }
    
    public void Update(string title, decimal price, string description, Guid categoryId, string image, Rating rating)
    {
        Title = title;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        Image = image;
        Rating = rating;
    }
    
}