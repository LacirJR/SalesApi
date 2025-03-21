using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Products.Application.Commands.UpdateProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProductCommand> _validator;
    private readonly IMapper _mapper;
    private readonly UpdateProductCommandHandler _handler;
    private readonly Faker _faker;

    public UpdateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<UpdateProductCommand>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new UpdateProductCommandHandler(_productRepository, _categoryRepository, _unitOfWork, _validator,
            _mapper);
        _faker = new Faker();
    }

    [Fact]
    public async Task Should_Update_Product_Successfully()
    {
        var productId = Guid.NewGuid();
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            category.Id,
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        var command = new UpdateProductCommand(
            productId,
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            category.Name,
            _faker.Image.PicsumUrl(),
            new RatingDto(4.8m, 15)
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        _categoryRepository.GetByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _mapper.Map<ProductResponseDto>(Arg.Any<Product>())
            .Returns(new ProductResponseDto { Title = command.Title, Price = command.Price });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(command.Title, result.Data.Title);

        _productRepository.Received(1).Update(Arg.Any<Product>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new UpdateProductCommand(Guid.Empty, "", 0, "", "", "", new RatingDto(0, 0));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "Title is required"),
                new ValidationFailure("Price", "Price must be greater than zero")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        _productRepository.DidNotReceive().Update(Arg.Any<Product>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, "New Title", 100, "New Description", "Category", "New Image",
            new RatingDto(5, 20));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);

        _productRepository.DidNotReceive().Update(Arg.Any<Product>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Category_Not_Found()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, "New Title", 100, "New Description", "NonExistentCategory",
            "New Image", new RatingDto(5, 20));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Product("Old Title", 50, "Old Description", Guid.NewGuid(), "Old Image",
                new Rating(4, 10))));

        _categoryRepository.GetByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error.Type);
        Assert.Equal("Category not found.", result.Error.Detail);

        _productRepository.DidNotReceive().Update(Arg.Any<Product>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_UnitOfWork_Fails()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, "New Title", 100, "New Description", "Category", "New Image",
            new RatingDto(5, 20));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Product("Old Title", 50, "Old Description", Guid.NewGuid(), "Old Image",
                new Rating(4, 10))));

        _categoryRepository.GetByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(new Category("Test")));

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        _productRepository.Received(1).Update(Arg.Any<Product>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}