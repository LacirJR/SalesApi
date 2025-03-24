using Bogus;
using Microsoft.EntityFrameworkCore;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using Module.Sales.Domain.Enums;
using Module.Sales.Infrastructure.Persistence.Repositories;
using Module.Sales.Tests.Unit.Fakes;
using Module.Sales.Tests.Unit.Fixtures;
using NSubstitute;
using Xunit;


namespace Module.Sales.Tests.Unit.Repositories;

public class SaleRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly FakeSaleDbContext _context;
    private readonly ISaleRepository _repository;

    public SaleRepositoryTests(DbContextFixture fixture)
    {
        _context = fixture.CreateNewContext();
        _repository = new SaleRepository(_context);
    }

    private Sale GenerateFakeSale()
    {
        var faker = new Faker("pt_BR");

        var sale = new Sale(faker.Date.Recent(), Guid.NewGuid(), faker.Company.CompanyName(), Guid.NewGuid());
        
        var items = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(
                sale.Id,
                Guid.NewGuid(),
                f.Random.Int(1, 20),
                f.Random.Decimal(0,20),
                f.Random.Decimal(0,20)
            ))
            .Generate(2);
        
        foreach (var item in items)
            sale.AddItem(item);

        return sale;
    }

    [Fact]
    public async Task AddAsync_Should_Add_Sale()
    {
        var sale = GenerateFakeSale();

        await _repository.AddAsync(sale, CancellationToken.None);
        await _context.SaveChangesAsync(Arg.Any<CancellationToken>());

        var dbSale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        Assert.NotNull(dbSale);
    }

    [Fact]
    public async Task Update_Should_Update_Sale_When_Tracked()
    {
        var sale = GenerateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        sale.Finish(); 
        _repository.Update(sale);
        await _context.SaveChangesAsync();

        var updatedSale = await _context.Sales.FindAsync(sale.Id);
        Assert.Equal(SaleStatus.Finalized, updatedSale!.Status);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Sale_With_Items()
    {
        var sale = GenerateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(sale.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(sale.Id, result!.Id);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetByNumberAsync_Should_Return_Sale()
    {
        var sale = GenerateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByNumberAsync(sale.Number, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(sale.Number, result!.Number);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Paginated_List()
    {
        for (int i = 0; i < 10; i++)
        {
            var sale = GenerateFakeSale();
            await _context.Sales.AddAsync(sale);
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync(null, "Number", 1, 5, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.Data.ToList().Count);
        Assert.True(result.TotalItems >= 10);
    }
}
