using Bogus;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Infrastructure.Persistence.Repositories;
using Module.Products.Tests.Unit.Fakes;
using Module.Products.Tests.Unit.Fixtures;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Repositories;

public class CategoryRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly IProductDbContext _dbContext;
    private readonly ICategoryRepository _repository;
    private readonly Faker _faker;

    public CategoryRepositoryTests(DbContextFixture fixture)
    {
        _dbContext = fixture.CreateNewContext();;
        _repository = new CategoryRepository(_dbContext);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Return_True_When_Category_Exists()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var category = new Category(categoryName);

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.ExistsByNameAsync(categoryName, CancellationToken.None);

        Assert.True(result);
    }
    
    [Fact]
    public async Task Should_Return_False_When_Category_Does_Not_Exist()
    {
        var result = await _repository.ExistsByNameAsync("NonExistentCategory", CancellationToken.None);
        Assert.False(result);
    }
    
    [Fact]
    public async Task Should_Return_Category_When_Found_By_Name()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var category = new Category(categoryName);

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.GetByNameAsync(categoryName, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);
    }
    
    [Fact]
    public async Task Should_Return_Null_When_Category_Not_Found()
    {
        var result = await _repository.GetByNameAsync("NonExistentCategory", CancellationToken.None);
        Assert.Null(result);
    }
    
    
    [Fact]
    public async Task Should_Add_Category()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);

        await _repository.AddAsync(category, CancellationToken.None);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task Should_Remove_Category_When_Exists()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var category = new Category(categoryName);

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.RemoveByNameAsync(categoryName, CancellationToken.None);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        Assert.Equal(categoryName, result.Name);

        var existsAfterRemoval = await _dbContext.Categories.AnyAsync(c => c.Name == categoryName);
        Assert.False(existsAfterRemoval);
    }
    
    [Fact]
    public async Task Should_Return_Null_When_Removing_Nonexistent_Category()
    {
        var result = await _repository.RemoveByNameAsync("NonExistentCategory", CancellationToken.None);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Should_Get_All_Categories()
    {
        
        var resultActual = await _repository.GetAllAsync(CancellationToken.None);
        
        var categories = new List<Category>
        {
            new(_faker.Commerce.Categories(1)[0]),
            new(_faker.Commerce.Categories(1)[0])
        };

        await _dbContext.Categories.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.GetAllAsync(CancellationToken.None);

        Assert.Equal(resultActual.Count + categories.Count, result.Count);
    }
    
}