using Bogus;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using Module.Products.Infrastructure.Persistence.Repositories;
using Module.Products.Tests.Unit.Fixtures;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Repositories;

public class ProductRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly IProductDbContext _dbContext;
    private readonly IProductRepository _repository;
    private readonly Faker _faker;

    public ProductRepositoryTests(DbContextFixture fixture)
    {
        _dbContext = fixture.CreateNewContext();;
        _repository = new ProductRepository(_dbContext);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Add_Product_Successfully()
    {
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 1000),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        await _repository.AddAsync(product, CancellationToken.None);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var exists = await _dbContext.Products.AnyAsync(p => p.Title == product.Title);
        Assert.True(exists);
    }
    
    [Fact]
    public async Task Should_Update_Product_Successfully()
    {
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 1000),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        product.Update("Updated Title", 999, "Updated Description", product.CategoryId, product.Image, new Rating(5.0m, 20));

        _repository.Update(product);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var updatedProduct = await _dbContext.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Title", updatedProduct.Title);
        Assert.Equal(999, updatedProduct.Price);
    }
    
    [Fact]
    public async Task Should_Remove_Product_By_Id()
    {
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 1000),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.RemoveByIdAsync(product.Id, CancellationToken.None);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.NotNull(result);
        var exists = await _dbContext.Products.AnyAsync(p => p.Id == product.Id);
        Assert.False(exists);
    }
    
     [Fact]
    public async Task Should_Return_Null_When_Removing_Nonexistent_Product()
    {
        var result = await _repository.RemoveByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Should_Check_If_Product_Exists_By_Title_And_Category()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var product = new Product(
            "Unique Product",
            100,
            "Test",
            category.Id,
            _faker.Image.PicsumUrl(),
            new Rating(5, 50)
        );

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var exists = await _repository.ExistsByTitleAndCategoryAsync("Unique Product", category.Name, CancellationToken.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task Should_Return_False_When_Category_Does_Not_Exist()
    {
        var exists = await _repository.ExistsByTitleAndCategoryAsync("Nonexistent Product", "Nonexistent Category", CancellationToken.None);
        Assert.False(exists);
    }

    [Fact]
    public async Task Should_Check_If_Any_Products_Exist_By_Category_Id()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 1000),
            _faker.Lorem.Sentence(),
            category.Id,
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var exists = await _repository.AnyByCategoryIdAsync(category.Id, CancellationToken.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task Should_Get_Product_By_Id()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        
        var product = new Product(
            "Test Product",
            500,
            "Test Description",
            category.Id,
            "test.jpg",
            new Rating(4.8m, 100)
        );

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(default);

        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(product.Title, result.Title);
    }

    [Fact]
    public async Task Should_Return_Null_When_Product_Not_Found_By_Id()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Should_Return_Paginated_Products()
    {
        var category = new Category("Tech");
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        for (int i = 0; i < 15; i++)
        {
            var product = new Product(
                _faker.Commerce.ProductName(),
                _faker.Random.Decimal(1, 1000),
                _faker.Lorem.Sentence(),
                category.Id,
                _faker.Image.PicsumUrl(),
                new Rating(4.5m, 10)
            );
            await _dbContext.Products.AddAsync(product);
        }

        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.GetAllAsync(null, "title asc", 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.ToList().Count);
        Assert.Equal(15, result.TotalItems);
    }
    
}