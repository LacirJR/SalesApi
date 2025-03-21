using Module.Products.Domain.Exceptions;
using Shared.Domain.Common;

namespace Module.Products.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; private set; }

    private Category() { }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(name))
            throw new ProductDomainException("O nome da categoria é obrigatório.");

        Name = name;
    }
}