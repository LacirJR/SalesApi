using System.ComponentModel.DataAnnotations.Schema;
using Module.Sales.Domain.Enums;
using Module.Sales.Domain.Events;
using Module.Sales.Domain.Exceptions;
using Shared.Domain.Common;

namespace Module.Sales.Domain.Entities;

public sealed class Sale : BaseEntity
{
    public long Number { get; private set; }
    public DateTime Date { get; private set; }
    public Guid UserId { get; private set; }
    public string Branch { get; private set; }
    
    [NotMapped]
    public bool IsCanceled => Status == SaleStatus.Canceled;
    
    [NotMapped]
    public bool IsFinalized => Status == SaleStatus.Finalized;
    public SaleStatus Status { get; private set; }
    

    public Guid CartId { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public decimal TotalValue => _items.Sum(i => i.Total);

    private Sale() { }

    public Sale(DateTime date, Guid userId, string branch, Guid cartId)
    {
        Id = Guid.NewGuid();
        Date = date;
        UserId = userId;
        Branch = branch;
        CartId = cartId;
        Status = SaleStatus.Active;

        AddDomainEvent(new SaleCreatedEvent(Id, CartId));
    }

    public void AddItem(SaleItem item)
    {
        _items.Add(item);
    }
    
    public void CancelItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) throw new SaleDomainException("Item not found");

        _items.Remove(item);
        
        if(_items.Count == 0)
            Cancel();
        
        AddDomainEvent(new ItemCanceledEvent(Id, productId));
    }
    
    
    public void Cancel()
    {
        Status = SaleStatus.Canceled;
        AddDomainEvent(new SaleCanceledEvent(Id));
    }
    
    public void Finish()
    {
        Status = SaleStatus.Finalized;
        AddDomainEvent(new SaleFinalizedEvent(Id));
    }
    
    public void UpdateBranchAndDate(string branch, DateTime date)
    {
        if (IsCanceled)
            throw new SaleDomainException("Cannot update a canceled sale.");

        Branch = branch;
        Date = date;
        
        AddDomainEvent(new SaleModifiedEvent(Id));
    }
}