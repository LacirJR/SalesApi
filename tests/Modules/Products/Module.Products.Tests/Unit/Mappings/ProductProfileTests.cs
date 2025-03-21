using AutoMapper;
using Bogus;
using Module.Products.Application.Dtos;
using Module.Products.Application.Mappings;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using Xunit;

namespace Module.Products.Tests.Unit.Mappings;

public class ProductProfileTests
{
    private readonly IMapper _mapper;
    private readonly Faker _faker;

    public ProductProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfile>());
        _mapper = configuration.CreateMapper();
        _faker = new Faker();
    }
    
    [Fact]
    public void Should_Map_Product_To_ProductResponseDto()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var rating = new Rating(_faker.Random.Decimal(1, 5), _faker.Random.Int(1, 500));
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(10, 500),
            _faker.Lorem.Sentence(),
            category.Id,
            _faker.Image.PicsumUrl(),
            rating
        );
        

        var productDto = _mapper.Map<ProductResponseDto>(product);

        Assert.Equal(product.Title, productDto.Title);
        Assert.Equal(product.Price, productDto.Price);
        Assert.Equal(product.Description, productDto.Description);
        Assert.Equal(product.Image, productDto.Image);
        Assert.Equal(product.Rating.Rate, productDto.Rating.Rate);
        Assert.Equal(product.Rating.Count, productDto.Rating.Count);
    }
    
    [Fact]
    public void Should_Have_Valid_AutoMapper_Configuration()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfile>());
        configuration.AssertConfigurationIsValid();
    }
}