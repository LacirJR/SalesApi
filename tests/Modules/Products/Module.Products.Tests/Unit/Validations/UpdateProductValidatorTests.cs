using Bogus;
using FluentValidation.TestHelper;
using Module.Products.Application.Commands.UpdateProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Application.Validations;
using NSubstitute;
using Xunit;

namespace Module.Products.Tests.Unit.Validations;

public class UpdateProductValidatorTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly UpdateProductValidator _validator;
    private readonly Faker<UpdateProductCommand> _fakerCommand;

    public UpdateProductValidatorTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _validator = new UpdateProductValidator(_productRepository, _categoryRepository);

        _fakerCommand = new Faker<UpdateProductCommand>()
            .CustomInstantiator(f => new UpdateProductCommand(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                f.Random.Decimal(1, 1000),
                f.Lorem.Sentence(),
                f.Commerce.Categories(1)[0],
                f.Internet.Url(),
                new RatingDto(f.Random.Decimal(0, 5), f.Random.Int(0, 500))
            ));
    }

    [Fact]
    public async Task Should_Validate_Successfully_When_Product_Is_Valid()
    {
        var command = _fakerCommand.Generate();

        _categoryRepository.ExistsByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _productRepository.ExistsByTitleAndCategoryAsync(command.Title, command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = _fakerCommand.Generate();
        command = command with { Id = Guid.Empty };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }

    [Fact]
    public async Task Should_Fail_When_Title_Is_Empty()
    {
        var command = _fakerCommand.Generate();
        command = command with { Title = "" };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("The product title is required.");
    }

    [Fact]
    public async Task Should_Fail_When_Title_Exceeds_Max_Length()
    {
        var command = _fakerCommand.Generate();
        command = command with { Title = new string('A', 256) };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("The title must not exceed 255 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_Price_Is_Zero_Or_Less()
    {
        var command = _fakerCommand.Generate();
        command = command with { Price = 0 };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("The product price must be greater than zero.");
    }

    [Fact]
    public async Task Should_Fail_When_Description_Is_Empty()
    {
        var command = _fakerCommand.Generate();
        command = command with { Description = "" };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("The product description is required.");
    }

    [Fact]
    public async Task Should_Fail_When_Description_Exceeds_Max_Length()
    {
        var command = _fakerCommand.Generate();
        command = command with { Description = new string('A', 1001) };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("The description must not exceed 1000 characters.");
    }

    [Fact]
    public async Task Should_Fail_When_Category_Does_Not_Exist()
    {
        var command = _fakerCommand.Generate();

        _categoryRepository.ExistsByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("The specified category does not exist.");
    }
    

    [Fact]
    public async Task Should_Fail_When_Image_Url_Is_Invalid()
    {
        var command = _fakerCommand.Generate();
        command = command with { Image = "invalid-url" };

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Image)
            .WithErrorMessage("The image URL is invalid.");
    }

    [Fact]
    public async Task Should_Validate_When_Image_Url_Is_Valid()
    {
        var command = _fakerCommand.Generate();

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Image);
    }
}