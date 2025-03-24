using Bogus;
using FluentValidation.TestHelper;
using Module.Products.Application.Commands.CreateCategoryCommand;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Validations;
using Module.Products.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Validations;

public class CreateCategoryValidatorTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CreateCategoryValidator _validator;
    private readonly Faker _faker;

    public CreateCategoryValidatorTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _validator = new CreateCategoryValidator(_categoryRepository);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Validate_Successfully_When_Category_Is_Valid()
    {
        var command = new CreateCategoryCommand(_faker.Commerce.Categories(1)[0]);

        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public async Task Should_Fail_When_CategoryName_Is_Empty()
    {
        var command = new CreateCategoryCommand("");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryName)
            .WithErrorMessage("The category name is required.");
    }
    
    [Fact]
    public async Task Should_Fail_When_Category_Already_Exists()
    {
        var existingCategory = new Category(_faker.Commerce.Categories(1)[0]);
        var command = new CreateCategoryCommand(existingCategory.Name);

        _categoryRepository.GetByNameAsync(existingCategory.Name, default)
            .Returns(Task.FromResult(existingCategory));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryName)
            .WithErrorMessage("The specified category already exist.");
    }
}