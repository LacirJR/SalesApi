using AutoMapper;
using Bogus;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Queries.GetProductsQuery;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using NSubstitute;
using Shared.Infrastructure.Common;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductsQueryHandler _handler;
    private readonly Faker _faker;

    public GetProductsQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetProductsQueryHandler(_productRepository, _mapper);
        _faker = new Faker();
    }

    [Fact]
    public async Task Should_Return_Paginated_Products_Successfully()
    {
        var query = new GetAllProductsQuery()
        {
            Filter = "price > 50",
            OrderBy = "title ASC",
            Page = 1,
            Size = 10,
        };

        var products = new List<Product>
        {
            new(_faker.Commerce.ProductName(), _faker.Random.Decimal(50, 200), _faker.Lorem.Sentence(),
                Guid.NewGuid(), _faker.Image.PicsumUrl(), new Rating(4.5m, 10)),
            new(_faker.Commerce.ProductName(), _faker.Random.Decimal(50, 200), _faker.Lorem.Sentence(),
                Guid.NewGuid(), _faker.Image.PicsumUrl(), new Rating(4.8m, 20))
        };

        var paginatedProducts = new PaginatedList<Product>(products, products.Count, query.Page, query.Size);

        _productRepository
            .GetAllAsync(query.Filter, query.OrderBy, query.Page, query.Size, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedProducts));

        var productsDto = new List<ProductResponseDto>
        {
            new() { Title = products[0].Title, Price = products[0].Price },
            new() { Title = products[1].Title, Price = products[1].Price }
        };

        _mapper.Map<List<ProductResponseDto>>(products)
            .Returns(productsDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(productsDto.Count, result.Data.Data.ToList().Count);
        Assert.Equal(query.Page, result.Data.CurrentPage);

        await _productRepository.Received(1).GetAllAsync(query.Filter, query.OrderBy, query.Page, query.Size,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_No_Products_Exist()
    {
        var query = new GetAllProductsQuery();

        var paginatedProducts = new PaginatedList<Product>(new List<Product>(), 0, query.Page, query.Size);

        _productRepository
            .GetAllAsync(query.Filter, query.OrderBy, query.Page, query.Size, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedProducts));

        _mapper.Map<List<ProductResponseDto>>(Arg.Any<List<Product>>())
            .Returns(new List<ProductResponseDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Data.Data);

        await _productRepository.Received(1).GetAllAsync(query.Filter, query.OrderBy, query.Page, query.Size,
            Arg.Any<CancellationToken>());
    }
}