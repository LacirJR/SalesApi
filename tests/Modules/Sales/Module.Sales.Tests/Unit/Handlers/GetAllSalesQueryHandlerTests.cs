using AutoMapper;
using Bogus;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Application.Mappings;
using Module.Sales.Application.Queries.GetAllSalesQuery;
using Module.Sales.Domain.Entities;
using NSubstitute;
using Shared.Infrastructure.Common;
using Xunit;

namespace Module.Sales.Tests.Unit.Handlers;

public class GetAllSalesQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper;
    private readonly GetAllSalesQueryHandler _handler;
    private readonly Faker _faker = new();

    public GetAllSalesQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SaleProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetAllSalesQueryHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Should_Return_Paginated_Sales_List()
    {
        var query = new GetAllSalesQuery(string.Empty, "date desc", 1,10);

        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        var saleItem = new SaleItem(sale.Id, Guid.NewGuid(), 2, 100, 10);
        sale.AddItem(saleItem);

        var paginatedSales = new PaginatedList<Sale>(
            new List<Sale> { sale }, 1, query.Page, query.Size);

        _saleRepository.GetAllAsync(query.Filter, query.OrderBy, query.Page, query.Size, Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Data);
        Assert.Equal(sale.Id, result.Data.Data.First().Id);
    }
}
