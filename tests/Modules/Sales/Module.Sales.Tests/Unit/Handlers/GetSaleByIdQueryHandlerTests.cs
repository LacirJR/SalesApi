using AutoMapper;
using Bogus;
using FluentValidation;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Application.Mappings;
using Module.Sales.Application.Queries.GetSaleByIdQuery;
using Module.Sales.Domain.Entities;
using NSubstitute;
using Shared.Domain.Common;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Module.Sales.Tests.Unit.Handlers;

public class GetSaleByIdQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper;
    private readonly IValidator<GetSaleByIdQuery> _validator = Substitute.For<IValidator<GetSaleByIdQuery>>();
    private readonly GetSaleByIdQueryHandler _handler;
    private readonly Faker _faker = new();

    public GetSaleByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SaleProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetSaleByIdQueryHandler(_saleRepository, _mapper, _validator);
    }

    [Fact]
    public async Task Should_Return_Sale_When_Exists()
    {
        var query = new GetSaleByIdQuery(Guid.NewGuid());

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        sale.AddItem(new SaleItem(sale.Id, Guid.NewGuid(), 1, 100, 5));

        _saleRepository.GetByIdAsync(query.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(sale.Id, result.Data.Id);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Sale_Does_Not_Exist()
    {
        var query = new GetSaleByIdQuery(Guid.NewGuid());

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(query.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Type, result.Error?.Type);
    }
}
