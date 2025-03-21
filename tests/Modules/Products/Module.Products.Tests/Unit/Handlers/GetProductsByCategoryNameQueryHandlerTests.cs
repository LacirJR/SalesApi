using AutoMapper;
using Bogus;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Queries.GetProductsByCategoryNameQuery;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using NSubstitute;
using Shared.Infrastructure.Common;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class GetProductsByCategoryNameQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly GetProductsByCategoryNameQueryHandler _handler;
    private readonly Faker _faker;

    public GetProductsByCategoryNameQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetProductsByCategoryNameQueryHandler(_productRepository, _mapper, _categoryRepository);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Return_Paginated_Products_Successfully()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var category = new Category(categoryName);
        var query = new GetProductsByCategoryNameQuery(categoryName, "title ASC", 1, 10);

        var products = new List<Product>
        {
            new (_faker.Commerce.ProductName(), _faker.Random.Decimal(1, 100), _faker.Lorem.Sentence(), category.Id, _faker.Image.PicsumUrl(), new Rating(4.5m, 10)),
            new (_faker.Commerce.ProductName(), _faker.Random.Decimal(1, 100), _faker.Lorem.Sentence(), category.Id, _faker.Image.PicsumUrl(), new Rating(4.8m, 20))
        };

        var paginatedProducts = new PaginatedList<Product>(products, products.Count, query.Page, query.Size);

        _categoryRepository.GetByNameAsync(categoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _productRepository.GetAllAsync($"categoryId={category.Id}", query.Order, query.Page, query.Size, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedProducts));

        var productsDto = new List<ProductResponseDto>
        {
            new () { Title = products[0].Title, Price = products[0].Price },
            new () { Title = products[1].Title, Price = products[1].Price }
        };

        _mapper.Map<List<ProductResponseDto>>(products)
            .Returns(productsDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(productsDto.Count, result.Data.Data.ToList().Count);
        Assert.Equal(query.Page, result.Data.PageNumber);

        await _categoryRepository.Received(1).GetByNameAsync(categoryName, Arg.Any<CancellationToken>());
        await _productRepository.Received(1).GetAllAsync($"categoryId={category.Id}", query.Order, query.Page, query.Size, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Category_Not_Found()
    {
        var categoryName = "NonExistentCategory";
        var query = new GetProductsByCategoryNameQuery(categoryName, "title ASC", 1, 10);

        _categoryRepository.GetByNameAsync(categoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null));

        
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error.Type);
        Assert.Equal("Category not found.", result.Error.Detail);

        await _categoryRepository.Received(1).GetByNameAsync(categoryName, Arg.Any<CancellationToken>());
        await _productRepository.DidNotReceive().GetAllAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}