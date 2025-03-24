using AutoMapper;
using Bogus;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Mappings;
using Module.Sales.Domain.Entities;
using Xunit;

namespace Module.Sales.Tests.Unit.Mappings;

public class SaleProfileTests
{
    private readonly IMapper _mapper;
    private readonly Faker _faker = new();

    public SaleProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SaleProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_Sale_To_SaleResponseDto()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        var item1 = new SaleItem(sale.Id, Guid.NewGuid(), 2, 100, 10);
        var item2 = new SaleItem(sale.Id, Guid.NewGuid(), 1, 200, 0);
        sale.AddItem(item1);
        sale.AddItem(item2);

        var dto = _mapper.Map<SaleResponseDto>(sale);

        Assert.Equal(sale.Id, dto.Id);
        Assert.Equal(sale.UserId, dto.UserId);
        Assert.Equal(sale.CartId, dto.CartId);
        Assert.Equal(sale.Branch, dto.Branch);
        Assert.Equal(sale.Date, dto.Date);
        Assert.Equal(sale.TotalValue, dto.TotalValue);
        Assert.Equal(sale.Items.Count, dto.Items.Count);
    }

    [Fact]
    public void Should_Map_SaleItem_To_SaleItemDto()
    {
        var item = new SaleItem(Guid.NewGuid(), Guid.NewGuid(), 3, 50, 20);

        var dto = _mapper.Map<SaleItemDto>(item);

        Assert.Equal(item.ProductId, dto.ProductId);
        Assert.Equal(item.Quantity, dto.Quantity);
        Assert.Equal((double)item.DiscountPercentage, dto.DiscountPercentage);
        Assert.Equal(item.Total, dto.Total);
    }
}
