using AutoMapper;
using Bogus;
using FluentValidation;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Application.Mappings;
using Module.Sales.Application.Queries.GetSaleByNumberQuery;
using Module.Sales.Domain.Entities;
using NSubstitute;
using Shared.Domain.Common;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Module.Sales.Tests.Unit.Handlers;

public class GetSaleByNumberQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IValidator<GetSaleByNumberQuery> _validator = Substitute.For<IValidator<GetSaleByNumberQuery>>();
    private readonly IMapper _mapper;
    private readonly GetSaleByNumberQueryHandler _handler;
    private readonly Faker _faker = new();

    public GetSaleByNumberQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SaleProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetSaleByNumberQueryHandler(_saleRepository, _mapper, _validator);
    }

    [Fact]
    public async Task Should_Return_Sale_When_Number_Is_Valid()
    {
        var query = new GetSaleByNumberQuery(_faker.Random.Long(1, 99999));

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        sale.AddItem(new SaleItem(sale.Id, Guid.NewGuid(), 2, 150, 10));

        typeof(Sale)
            .GetProperty(nameof(Sale.Number))!
            .SetValue(sale, query.Number);

        _saleRepository.GetByNumberAsync(query.Number, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(query.Number, result.Data.Number);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Sale_Does_Not_Exist()
    {
        var query = new GetSaleByNumberQuery(_faker.Random.Long(1, 99999));

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByNumberAsync(query.Number, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Type, result.Error?.Type);
    }
}
