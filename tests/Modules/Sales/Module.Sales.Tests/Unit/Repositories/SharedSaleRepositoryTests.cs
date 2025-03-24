
using Bogus;
using Microsoft.EntityFrameworkCore;
using Module.Sales.Domain.Entities;
using Module.Sales.Domain.Enums;
using Module.Sales.Infrastructure.Persistence.Repositories;
using Module.Sales.Tests.Unit.Fakes;
using Module.Sales.Tests.Unit.Fixtures;
using Shared.Application.Interfaces.Persistence;
using Xunit;

namespace Module.Sales.Tests.Unit.Repositories;

public class SharedSaleRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly FakeSaleDbContext _context;
    private readonly ISharedSaleRepository _repository;

    public SharedSaleRepositoryTests(DbContextFixture fixture)
    {
        _context = fixture.CreateNewContext();
        _repository = new SharedSaleRepository(_context);
    }

    private (Sale Sale, Guid ProductId) CreateSaleWithProduct()
    {
        var faker = new Faker();
        var productId = Guid.NewGuid();


        var sale = new Sale(faker.Date.Recent(),Guid.NewGuid(), faker.Company.CompanyName(), Guid.NewGuid());
        var item = new SaleItem(sale.Id, productId, 1, 100.0m, 10m);
        sale.AddItem(item);

        return (sale, productId);
    }

    [Fact]
    public async Task RemoveProductByIdAsync_Should_Cancel_Item_When_Sale_Is_Active()
    {
        var (sale, productId) = CreateSaleWithProduct();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        await _repository.RemoveProductByIdAsync(productId, CancellationToken.None);

        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        var removedItem = updatedSale?.Items.FirstOrDefault(i => i.ProductId == productId);

        Assert.NotNull(updatedSale);
        Assert.Null(removedItem);
        Assert.False(updatedSale.Items.Any(i => i.ProductId == productId));

    }

    [Fact]
    public async Task RemoveProductByIdAsync_Should_Not_Cancel_Item_When_Sale_Is_Not_Active()
    {
        var (sale, productId) = CreateSaleWithProduct();
        sale.Finish();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

  
        await _repository.RemoveProductByIdAsync(productId, CancellationToken.None);
        await _context.SaveChangesAsync();
        
        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        var item = updatedSale?.Items.FirstOrDefault(i => i.ProductId == productId);

        Assert.NotNull(updatedSale);
        Assert.NotNull(item);
        Assert.True(updatedSale.Items.Any(i => i.ProductId == productId));
    }

    [Fact]
    public async Task RemoveProductByIdAsync_Should_Do_Nothing_When_Product_Not_Found()
    {
        var unrelatedProductId = Guid.NewGuid();

        var (sale, _) = CreateSaleWithProduct();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        await _repository.RemoveProductByIdAsync(unrelatedProductId, CancellationToken.None);

        var unchangedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        Assert.Equal(unchangedSale!.Items.Count, sale.Items.Count);

    }
}
