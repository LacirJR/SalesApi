using AutoMapper;
using Bogus;
using Module.Products.Application.Dtos;
using Module.Products.Application.Mappings;
using Module.Products.Domain.Entities;
using Xunit;

namespace Module.Products.Tests.Unit.Mappings;

public class CategoryProfileTests
{
    private readonly IMapper _mapper;
    private readonly Faker _faker;

    public CategoryProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>());
        _mapper = configuration.CreateMapper();
        _faker = new Faker();
    }
    
    [Fact]
    public void Should_Map_Category_To_CategoryResponseDto()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);

        var categoryDto = _mapper.Map<CategoryResponseDto>(category);

        Assert.Equal(category.Name, categoryDto.Name);
    }
    
    [Fact]
    public void Should_Have_Valid_AutoMapper_Configuration()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>());
        configuration.AssertConfigurationIsValid();
    }
}