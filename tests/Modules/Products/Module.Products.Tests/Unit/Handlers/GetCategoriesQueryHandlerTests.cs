using Bogus;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Queries.GetCategoriesQuery;
using Module.Products.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class GetCategoriesQueryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly GetCategoriesQueryHandler _handler;
    private readonly Faker _faker;

    public GetCategoriesQueryHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _handler = new GetCategoriesQueryHandler(_categoryRepository);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Return_All_Categories_Successfully()
    {
        var categories = new List<Category>
        {
            new Category(_faker.Commerce.Categories(1)[0]),
            new Category(_faker.Commerce.Categories(1)[0]),
            new Category(_faker.Commerce.Categories(1)[0])
        };

        _categoryRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(categories));

        var query = new GetCategoriesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(categories.Select(c => c.Name).ToList(), result.Data);
        Assert.NotEmpty(result.Data);

        await _categoryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Return_Empty_List_When_No_Categories_Exist()
    {
        _categoryRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Category>()));

        var query = new GetCategoriesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Data);

        await _categoryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }
}