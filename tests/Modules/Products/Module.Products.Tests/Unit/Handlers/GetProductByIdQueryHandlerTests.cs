using AutoMapper;
using Bogus;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Queries.GetProductByIdQuery;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using NSubstitute;
using Shared.Domain.Common;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class GetProductByIdQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductByIdQueryHandler _handler;
    private readonly Faker _faker;

    public GetProductByIdQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductByIdQueryHandler(_productRepository, _mapper);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Return_Product_Successfully()
    {
        var productId = Guid.NewGuid();
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        var command = new GetProductByIdQuery(productId);

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        var expectedResponse = new ProductResponseDto
        {
            Title = product.Title,
            Price = product.Price
        };

        _mapper.Map<ProductResponseDto>(product)
            .Returns(expectedResponse);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(product.Title, result.Data.Title);

        await _productRepository.Received(1).GetByIdAsync(productId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        var productId = Guid.NewGuid();
        var command = new GetProductByIdQuery(productId);

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);

        await _productRepository.Received(1).GetByIdAsync(productId, Arg.Any<CancellationToken>());
    }
    
}