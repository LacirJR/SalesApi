using Bogus;
using FluentValidation.TestHelper;
using Module.Products.Application.Commands.DeleteCategoryCommand;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Validations;
using Module.Products.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Validations;

public class DeleteCategoryValidatorTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly DeleteCategoryValidator _validator;
    private readonly Faker _faker;

    public DeleteCategoryValidatorTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _validator = new DeleteCategoryValidator(_categoryRepository);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Validate_Successfully_When_Category_Exists()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var command = new DeleteCategoryCommand(categoryName);

        _categoryRepository.GetByNameAsync(categoryName, default)
            .Returns(Task.FromResult(new Category(categoryName)));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_CategoryName_Is_Empty()
    {
        var command = new DeleteCategoryCommand("");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryName)
            .WithErrorMessage("The category name is required.");
    }

    [Fact]
    public async Task Should_Fail_When_Category_Does_Not_Exist()
    {
        var categoryName = "NonExistentCategory";
        var command = new DeleteCategoryCommand(categoryName);

        _categoryRepository.GetByNameAsync(categoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryName)
            .WithErrorMessage("The specified category does not exist.");
    }

}